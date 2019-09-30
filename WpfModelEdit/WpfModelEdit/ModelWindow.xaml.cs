using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfModelEdit
{
    /// <summary>
    /// A neat WPF window to edit a model class.
    /// </summary>
    public partial class ModelWindow : Window
    {
        private struct PropertyInfoBinding
        {
            public PropertyInfo PropertyInfo;
            public TextBox TextBox;
        }


        /// <summary>
        /// Edit a model
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="defaultValuesObject"></param>
        /// <param name="objectConstructor"></param>
        /// <param name="editCallback"></param>
        public ModelWindow(Type objectType, object defaultValuesObject, Func<object> objectConstructor, Action<object> editCallback) : this(objectType, objectConstructor, editCallback)
        {
            DefaultValuesObject = defaultValuesObject;
        }


        /// <summary>
        /// New model
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="objectConstructor"></param>
        /// <param name="saveCallback"></param>
        public ModelWindow(Type objectType, Func<object> objectConstructor, Action<object> saveCallback)
        {
            InitializeComponent();
            Loaded += AddCdWindow_Loaded;
            ObjectType = objectType;
            ObjectConstructor = objectConstructor;
            SaveCallback = saveCallback;
        }

        public string DisplayText { get; set; }
        public Type ObjectType { get; }
        public Func<object> ObjectConstructor { get; }
        public Action<object> SaveCallback { get; }
        public object DefaultValuesObject { get; }

        private PropertyInfoBinding[] _objectProperties;

        private Regex _textSplitterRegex = new Regex("([A-Z]+(?=$|[A-Z][a-z]|[0-9])|[A-Z]?[a-z]+|[0-9]+)", RegexOptions.Compiled);

        private void AddCdWindow_Loaded(object sender, RoutedEventArgs e)
        {
            btnSave.Click += BtnSave_Click;
            btnCancel.Click += BtnCancel_Click;

            if (!string.IsNullOrWhiteSpace(DisplayText))
            {
                ModelGrid.RowDefinitions.Add(new RowDefinition());

                var label = new TextBlock()
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(12),
                    FontSize = 16,
                    Text = DisplayText
                };
                ModelGrid.Children.Add(label);
                Grid.SetRow(label, ModelGrid.RowDefinitions.Count - 1);
                Grid.SetColumn(label, 0);
                Grid.SetColumnSpan(label, 2);
            }

            var propertyInfos = ObjectType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            _objectProperties = new PropertyInfoBinding[propertyInfos.Length];

            for (int i = 0; i < propertyInfos.Length; i++)
            {
                ModelGrid.RowDefinitions.Add(new RowDefinition());

                var label = new TextBlock()
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(12),
                    Text = GetName(propertyInfos[i])
                };
                ModelGrid.Children.Add(label);
                Grid.SetRow(label, ModelGrid.RowDefinitions.Count - 1);
                Grid.SetColumn(label, 0);

                TextBox textBox = null;
                // https://stackoverflow.com/a/863944/3492994
                if (propertyInfos[i].PropertyType.IsPrimitive || propertyInfos[i].PropertyType.Equals(typeof(string)))
                {
                    textBox = new TextBox()
                    {
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(12)
                    };
                    ModelGrid.Children.Add(textBox);
                    Grid.SetRow(textBox, ModelGrid.RowDefinitions.Count - 1);
                    Grid.SetColumn(textBox, 1);

                    if (DefaultValuesObject != null)
                    {
                        textBox.Text = propertyInfos[i].GetValue(DefaultValuesObject) + "";
                    }

                }

                // TODO: Nullable types
                // TODO: Enums
                // TODO: Objects
                // TODO: Structs
                // TODO: Arrays

                _objectProperties[i] = new PropertyInfoBinding()
                {
                    PropertyInfo = propertyInfos[i],
                    TextBox = textBox
                };
            }
        }

        private string GetName(PropertyInfo propertyInfo)
        {
            string name = propertyInfo.GetCustomAttributes<System.ComponentModel.DisplayNameAttribute>().FirstOrDefault()?.DisplayName;

            if (string.IsNullOrWhiteSpace(name))
            {
                var displayAttribute = propertyInfo.GetCustomAttributes().FirstOrDefault(a => a.GetType().Name == "DisplayAttribute");
                if (displayAttribute != null)
                {
                    name = displayAttribute.GetType().GetProperty("Name")?.GetValue(displayAttribute) as string;
                }
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                name = _textSplitterRegex.Replace(propertyInfo.Name, "$1 ");
            }

            return name;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var modelObject = this.ObjectConstructor.Invoke();

            for (int i = 0; i < _objectProperties.Length; i++)
            {
                if (_objectProperties[i].TextBox == null) continue;

                var propText = _objectProperties[i].TextBox.Text;

                if (!string.IsNullOrEmpty(propText))
                {
                    object propValue = Convert.ChangeType(propText, _objectProperties[i].PropertyInfo.PropertyType);

                    _objectProperties[i].PropertyInfo.SetValue(modelObject, propValue);
                }
            }

            SaveCallback.Invoke(modelObject);
            this.Close();
        }
    }
}
