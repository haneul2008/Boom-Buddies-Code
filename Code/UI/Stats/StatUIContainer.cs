using System.Collections.Generic;
using System.Linq;

namespace Code.UI.Stats
{
    public class StatUIContainer
    {
        private readonly List<StatUI> _statUIList;
        private readonly List<StatUIDataSO> _statUIDataList;

        public StatUIContainer(List<StatUIDataSO> statUIDataList, List<StatUI> statUIList)
        {
            _statUIDataList = statUIDataList;
            _statUIList = statUIList;
        }

        public void SetStatUI(StatEnum prevEnum, StatEnum newEnum)
        {
            StatUI targetUI = _statUIList.FirstOrDefault(stat => stat.StatUIData.statEnum == prevEnum);
            StatUIDataSO newData = _statUIDataList.FirstOrDefault(data => data.statEnum == newEnum);
            
            targetUI?.SetUp(newData);
        }

        public StatUI GetStatUI(StatEnum statEnum) => _statUIList.FirstOrDefault(ui => ui.StatUIData.statEnum == statEnum);

        public List<StatUI> GetAllStatUI() => _statUIList;
    }
}