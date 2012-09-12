using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using System.Windows.Media;
using OxyPlot;
using TradeLink.AppKit;

namespace DarkLight.Utilities
{
    public static class OxyPlotExtensions
    {
        public static OxyColor OxyForegroundColor = OxyColors.White;

        public static void SetColors(this Axis oxyAxis)
        {
            oxyAxis.TextColor = OxyForegroundColor;
            oxyAxis.MajorGridlineColor = OxyForegroundColor;
            oxyAxis.MinorGridlineColor = OxyForegroundColor;
            oxyAxis.AxislineColor = OxyForegroundColor;
            oxyAxis.TitleColor = OxyForegroundColor;
            oxyAxis.TicklineColor = OxyForegroundColor;
            oxyAxis.ExtraGridlineColor = OxyForegroundColor;
        }

        public static void SetColors(this PlotModel model)
        {
            model.LegendTitleColor = OxyForegroundColor;
            model.PlotAreaBorderColor = OxyForegroundColor;
            model.LegendTextColor = OxyForegroundColor;
            model.SubtitleColor = OxyForegroundColor;
            model.TextColor = OxyForegroundColor;
            model.TitleColor = OxyForegroundColor;
        }
    }

    public class PlottingUtilities
    {
        

        public static List<Color> GetColorList(int numberOfColors)
        {
            var colorList = new List<Color>();
            double angleStep = 360.0/Convert.ToDouble(numberOfColors);
            for(int i = 0; i < numberOfColors; i++)
            {
                var angle = angleStep*i;
                var color = ColorFromHSV(angle, 1.0, 1.0);
                colorList.Add(color);
            }
            return colorList;
        }

        public static List<System.Drawing.Color> GetLegacyColorList(int numberOfColors)
        {
            List<System.Drawing.Color> legacyColorList = new List<System.Drawing.Color>();
            var colorList = GetColorList(numberOfColors);
            foreach (var _color in colorList)
            {
                 legacyColorList.Add(System.Drawing.Color.FromArgb(_color.A, _color.R, _color.B, _color.G));
            }
            return legacyColorList;
        }

        public static Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            byte v = Convert.ToByte(value);
            byte p = Convert.ToByte(value * (1 - saturation));
            byte q = Convert.ToByte(value * (1 - f * saturation));
            byte t = Convert.ToByte(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return Color.FromArgb(255, t, p, v);
            else
                return Color.FromArgb(255, v, p, q);
        }

        public static List<PlottableProperty> GetAllPlottableValues(Type plottableType)
        {
            var plottableValueList = new List<PlottableProperty>(); 
            var memberInfos = GetFieldsAndProperties(plottableType, BindingFlags.Instance | BindingFlags.Public);
            
            foreach (var memberInfo in memberInfos)
            {
                switch (memberInfo.MemberType)
                {
                    case MemberTypes.Property:
                    {
                        var propertyInfo = memberInfo as PropertyInfo;
                        if (propertyInfo.PropertyType == typeof(int) ||
                            propertyInfo.PropertyType == typeof(double) ||
                            propertyInfo.PropertyType == typeof(decimal))
                        {
                            plottableValueList.Add(new PlottableProperty
                            {
                                PropertyName = propertyInfo.Name,
                                Selected = false,
                            });
                        }
                        break;
                    }
                    case MemberTypes.Field:
                    {
                        var fieldInfo = memberInfo as FieldInfo;
                        if (fieldInfo.FieldType == typeof(int) ||
                            fieldInfo.FieldType == typeof(double) ||
                            fieldInfo.FieldType == typeof(decimal))
                        {
                            plottableValueList.Add(new PlottableProperty
                            {
                                PropertyName = fieldInfo.Name,
                                Selected = false,
                            });
                        }
                        break;
                    }
                    default:
                    {
                        break;
                    }
                }
            }
            return plottableValueList;
        }

        //public static List<PlottableProperty> GetAllPlottableValues(ExpandoObject instance)
        //{
        //    var plottableValueList = new List<PlottableProperty>();
        //    var instanceDict = (IDictionary<string, object>) instance;
        //    foreach (var _key in instanceDict.Keys)
        //    {
        //        plottableValueList.Add(new PlottableProperty
        //        {
        //            PropertyName = _key,
        //            Selected = false,
        //        });
        //    }
        //    return plottableValueList;
        //}

        public static List<AdjustableProperty> GetAllAdjustableProperties(Type typeToAdjust)
        {
            var adjustableProperties = new List<AdjustableProperty>();
            var propertyInfos = typeToAdjust.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var propertyInfo in propertyInfos)
            {
                if (propertyInfo.PropertyType == typeof(int) ||
                    propertyInfo.PropertyType == typeof(double)||
                    propertyInfo.PropertyType == typeof(decimal))
                {
                    adjustableProperties.Add(new AdjustableProperty
                    {
                        PropertyName = propertyInfo.Name,
                        PropertyType = propertyInfo.PropertyType,
                        Selected = false,
                    });
                }
            }
            return adjustableProperties;
        }

        public static List<MemberInfo> GetFieldsAndProperties(Type type, BindingFlags bindingAttr)
        {
            List<MemberInfo> targetMembers = new List<MemberInfo>();

            targetMembers.AddRange(type.GetFields(bindingAttr));
            targetMembers.AddRange(type.GetProperties(bindingAttr));

            return targetMembers;
        }

        public static List<PlottablePoint> ToPlottable<T>(List<PlottableValue<T>> plottableValueList, PlottableProperty plottableProperty)
        {
            var pointList = new List<PlottablePoint>();
            foreach (var plottableValue in plottableValueList)
            {
                var result = plottableValue.Value;
                var instanceType = result.GetType();
                var memberInfos = instanceType.GetMember(plottableProperty.PropertyName);
                foreach (var memberInfo in memberInfos)
                {
                    switch (memberInfo.MemberType)
                    {
                        case MemberTypes.Property:
                        {
                            var propertyInfo = memberInfo as PropertyInfo;
                            var propertyValue = Convert.ToDouble(propertyInfo.GetValue(result, null));
                            pointList.Add(new PlottablePoint { X = plottableValue.X, Y = propertyValue });
                            break;
                        }
                        case MemberTypes.Field:
                        {
                            var fieldInfo = memberInfo as FieldInfo;
                            var propertyValue = Convert.ToDouble(fieldInfo.GetValue(result));
                            pointList.Add(new PlottablePoint { X = plottableValue.X, Y = propertyValue });
                            break;
                        }
                        default:
                        {
                            break;
                        }
                    }
                }
            }
            return pointList;
        }

        public static void CopyParameters<T1,T2>(T1 sourceInstance, T2 destinationInstance)
        {
            var sourceMemberInfos = GetFieldsAndProperties(sourceInstance.GetType(), BindingFlags.Instance | BindingFlags.Public);
            foreach (MemberInfo memberInfo in sourceMemberInfos)
            {
                if(memberInfo is FieldInfo)
                {
                    var sourceFieldInfo = memberInfo as FieldInfo;
                    FieldInfo destinationFieldInfo = destinationInstance.GetType().GetField(memberInfo.Name);
                    if (destinationFieldInfo != null)
                    {
                        destinationFieldInfo.SetValue(destinationInstance, sourceFieldInfo.GetValue(sourceInstance));
                    }
                }
                else if(memberInfo is PropertyInfo)
                {
                    var sourcePropertyInfo = memberInfo as PropertyInfo;
                    PropertyInfo destinationPropertyInfo = destinationInstance.GetType().GetProperty(memberInfo.Name);
                    if (destinationPropertyInfo != null && 
                        sourcePropertyInfo.CanRead &&
                        destinationPropertyInfo.CanWrite)
                    {
                        destinationPropertyInfo.SetValue(destinationInstance, sourcePropertyInfo.GetValue(sourceInstance, null), null);
                    }
                }
            }
        }

        public static List<KeyValuePair<string,string>> GetFieldAndPropertyValueList<T>(T instance)
        {
            var valueList = new List<KeyValuePair<string, string>>();
            var instanceType = instance.GetType();
            var memberInfos = GetFieldsAndProperties(instanceType, BindingFlags.Instance | BindingFlags.Public);
            foreach (var memberInfo in memberInfos)
            {
                switch (memberInfo.MemberType)
                {
                    case MemberTypes.Property:
                        {
                            var propertyInfo = memberInfo as PropertyInfo;
                            var propertyValue = Convert.ToString(propertyInfo.GetValue(instance, null));
                            valueList.Add(new KeyValuePair<string, string>(propertyInfo.Name,propertyValue));
                            break;
                        }
                    case MemberTypes.Field:
                        {
                            var fieldInfo = memberInfo as FieldInfo;
                            var propertyValue = Convert.ToString(fieldInfo.GetValue(instance));
                            valueList.Add(new KeyValuePair<string, string>(fieldInfo.Name,propertyValue));
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
            return valueList;
        }
    }
}
