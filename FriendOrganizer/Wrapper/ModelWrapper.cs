using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Linq;

namespace FriendOrganizer.UI.Wrapper
{
    /// <summary>
    /// Klasa opakowująca model, korzystamy z generyków przez co opakujemy każdy model. Wywołuje walidację. Ustawia propertisy.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ModelWrapper<T> : NotifyDataErrorInfoBase
    {
        public T Model;

        public ModelWrapper(T model)
        {
            Model = model;
        }

        protected virtual TValue GetValue<TValue>([CallerMemberName]string propertyName = null)
        {
            return (TValue) typeof(T).GetProperty(propertyName).GetValue(Model);
        }

        protected virtual void SetValue<TValue>(TValue value, [CallerMemberName]string propertyName = null)
        {
            typeof(T).GetProperty(propertyName).SetValue(Model, value);
            OnPropertyChanged(propertyName);
            ValidatePropertyInternal(propertyName, value);
        }

        /// <summary>
        /// Walidacja 2 etapowa: 1.Adnotacje 2.Custom error
        /// </summary>
        /// <param name="propertyName"></param>
        private void ValidatePropertyInternal(string propertyName, object currentValue)
        {
            ClearErrors(propertyName);
            ValidateDataAnnotations(propertyName, currentValue);            
            ValidateCustomErrors(propertyName);
        }

        private void ValidateDataAnnotations(string propertyName, object currentValue)
        {
            var context = new ValidationContext(Model) {MemberName = propertyName};
            var results = new List<ValidationResult>();

            Validator.TryValidateProperty(currentValue, context, results);
            results.ForEach(r => AddError(propertyName, r.ErrorMessage));
        }

        private void ValidateCustomErrors(string propertyName)
        {
            var errors = ValidateProperty(propertyName);
            if (errors == null) return;
            foreach (var error in errors)
            {
                AddError(propertyName, error);
            }
        }

        /// <summary>
        /// Przeładuj tę metodę jeśli chcesz dodać walidację swojego propertisa
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected virtual IEnumerable<string> ValidateProperty(string propertyName)
        {
            return null;
        }
    }
}