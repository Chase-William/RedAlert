using System;
using System.Collections.Generic;
using System.Text;

namespace RedAlertBot
{
    /// <summary>
    /// Manages the interaction between the <see cref="ImageRecorder"/> and <see cref="RedLearner.RedLearner"/>.
    /// </summary>
    public class ImagePredictor
    {
        public const string VAULT = "Vault";
        public const string VAULT_UI = "VaultUI";
        public const string FRIDGE = "Fridge";
        public const string FRIDGE_UI = "FridgeUI";

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<CurrentClassChangingEventArgs> CurrentClassChanging;

        public event EventHandler<UpdateTickEventArgs> UpdateTick;

        /// <summary>
        /// Learner that classifies images.
        /// </summary>
        private static RedLearner.RedLearner Learner { get; set; } = new RedLearner.RedLearner();

        private ImgClasses currentClass = ImgClasses.Default;
        public ImgClasses CurrentClass
        {
            get => currentClass;
            set
            {
                UpdateTick?.Invoke(this, new UpdateTickEventArgs(value));
                if (CurrentClass == value) return;
                CurrentClassChanging?.Invoke(this, new CurrentClassChangingEventArgs(CurrentClass, value));
                currentClass = value;
            }
        }

        public void PredictImage(string path)
        {
            var test = Learner.ClassifySingleImage(path);
            switch (test)
            {
                case VAULT:
                    CurrentClass = ImgClasses.Vault;
                    break;
                case VAULT_UI:
                    CurrentClass = ImgClasses.Vault_UI;
                    break;
                case FRIDGE:
                    CurrentClass = ImgClasses.Fridge;
                    break;
                case FRIDGE_UI:
                    CurrentClass = ImgClasses.Fridge_UI;
                    break;
                default:
                    break;
            }
        }
    }

    public class CurrentClassChangingEventArgs : EventArgs
    {
        public ImgClasses OldValue { get; set; }
        public ImgClasses NewValue { get; set; }

        private CurrentClassChangingEventArgs() { }
        public CurrentClassChangingEventArgs(ImgClasses _oldValue, ImgClasses _newValue) 
        {
            OldValue = _oldValue;
            NewValue = _newValue;
        }
    }

    public class UpdateTickEventArgs : EventArgs
    {
        public ImgClasses Value { get; set; }

        private UpdateTickEventArgs() { }
        public UpdateTickEventArgs(ImgClasses _value)
        {
            Value = _value;
        }
    }
    
    public enum ImgClasses
    {
        Default = 0,
        Vault,
        Vault_UI,
        Fridge,
        Fridge_UI
    }
}
