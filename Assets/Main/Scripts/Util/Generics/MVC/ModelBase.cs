using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main.Scripts.Util.Generic_Helpers.MVC
{
    public abstract class ModelBase
    {
        private ISaveData _saveData;
        private IConfigData _configData;

        public ModelBase(ISaveData saveData, IConfigData configData)
        {
            _saveData = saveData;
            _configData = configData;
        }


        public virtual void Save()
        {
            _saveData.Save();
        }

        public virtual void Load()
        {
            _saveData.Load();
        }
    }


    public interface IData
    {
        string DataPath { get; set; }
    }

    public interface ISaveData : IData
    {
        void Save();
        void Load();
    }

    public interface IConfigData : IData
    {
    
    }
}