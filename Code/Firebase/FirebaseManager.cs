using System;
using System.Collections.Generic;
using Code.ETC;
using Code.Managers;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Newtonsoft.Json;
using UnityEngine;

namespace Code.Firebase
{
    public class FirebaseManager
    {
        public DatabaseReference reference;
        private FirebaseAuth _auth;
        private FirebaseUser _currentUser;

        public void Initialize()
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                if (task.Result == DependencyStatus.Available)
                {
                    _auth = FirebaseAuth.DefaultInstance;
                    _currentUser = _auth.CurrentUser;
                    reference = FirebaseDatabase.DefaultInstance.RootReference;

                    GuestLogin();
                }
                else
                {
                    Debug.Log("로그인 실패");
                }
            });
        }

        private void GuestLogin()
        {
            if (_currentUser != null) return;

            _auth.SignInAnonymouslyAsync().ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("로그인 성공");
                    _currentUser = task.Result.User;
                }
                else if (task.IsCanceled || task.IsFaulted)
                {
                    Debug.Log($"게스트 로그인 실패 {task.Exception}");
                }
            });
        }

        public void ReadData(Action<DataCollection> onComplete = null)
        {
            if (_currentUser == null)
            {
                onComplete?.Invoke(new DataCollection());
                return;
            }

            reference.Child("USER").Child(_currentUser.UserId).Child("BaseData").GetValueAsync()
                .ContinueWithOnMainThread(task =>
                {
                    if (task.IsCompleted)
                    {
                        DataSnapshot snapshot = task.Result;

                        DataCollection dataCollection =
                            JsonConvert.DeserializeObject<DataCollection>(snapshot.GetRawJsonValue(),
                                JsonSetting.JsonSettings);

                        foreach (var data in dataCollection.dataList)
                        {
                            Debug.Log(data.data);
                        }

                        onComplete?.Invoke(dataCollection);
                    }
                });
        }

        public void SaveData(string json)
        {
            if (_currentUser == null)
            {
                Debug.LogError("저장 실패: 로그인된 유저가 없습니다.");
                return;
            }

            reference.Child("USER").Child(_currentUser.UserId).Child("BaseData").SetRawJsonValueAsync(json)
                .ContinueWithOnMainThread(task =>
                {
                    if (task.IsCompleted)
                    {
                        Debug.Log("데이터 저장 성공");
                    }
                    else
                    {
                        Debug.LogError("데이터 저장 실패: " + task.Exception);
                    }
                });
        }

        public void GetAllUserData(bool includeCurrentUser, Action<List<DataCollection>> onComplete = null)
        {
            if (_currentUser == null)
            {
                Debug.LogError("유저가 로그인되지 않았습니다.");
                onComplete?.Invoke(new List<DataCollection>());
                return;
            }

            reference.Child("USER").GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    List<DataCollection> resultList = new List<DataCollection>();

                    foreach (var userSnapshot in snapshot.Children)
                    {
                        string userId = userSnapshot.Key;
                
                        if (userId == _currentUser.UserId && includeCurrentUser == false)
                            continue;
                        
                        if (userSnapshot.HasChild("BaseData"))
                        {
                            string rawJson = userSnapshot.Child("BaseData").GetRawJsonValue();
                            try
                            {
                                var data = JsonConvert.DeserializeObject<DataCollection>(rawJson, JsonSetting.JsonSettings);
                                resultList.Add(data);
                            }
                            catch (Exception e)
                            {
                                Debug.LogError($"데이터 파싱 실패: {e}");
                            }
                        }
                    }

                    onComplete?.Invoke(resultList);
                }
                else
                {
                    Debug.LogError($"전체 유저 데이터 로딩 실패: {task.Exception}");
                    onComplete?.Invoke(new List<DataCollection>());
                }
            });
        }
    }
}