using MilasQuest.GameData;
using System.Collections.Generic;
using UnityEngine;

namespace MilasQuest.GameData
{
    //This should be later abstracted into a Database system for each data type
    public class CellDataProvider : Singleton<CellDataProvider>
    {
        [SerializeField] private CellTypeData[] datas;

        private Dictionary<int, CellTypeData> _datasById;

        protected override void DoAwake()
        {
            base.DoAwake();
            _datasById = new Dictionary<int, CellTypeData>();
            for (int i = 0; i < datas.Length; i++)
            {
                _datasById.Add(datas[i].id, datas[i]);
            }
        }

        public CellTypeData GetCellTypeData(int id)
        {
            if (_datasById.TryGetValue(id, out CellTypeData data))
                return data;
            else
            {
                Debug.LogError("No CellTypeData found with ID #" + id);
                return null;
            }
        }

        public CellTypeData GetCellTypeData(CELL_TYPES cellType)
        {
            if (_datasById.TryGetValue((int)cellType, out CellTypeData data))
                return data;
            else
            {
                Debug.LogError("No CellTypeData found with CELL_TYPE =" + cellType.ToString());
                return null;
            }
        }
    }
}