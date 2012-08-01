using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Media;

namespace DarkLight.Utilities
{
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

        public static List<PlottableValue> GetAllPlottableValues(Type plottableType)
        {
            var plottableValueList = new List<PlottableValue>();
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
                            plottableValueList.Add(new PlottableValue
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
                            plottableValueList.Add(new PlottableValue
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

        public static List<PlotPoint1D> ToPlottable(List<ResultPoint1D> resultsList, PlottableValue plottableValue)
        {
            var pointList = new List<PlotPoint1D>();
            foreach (var resultSet in resultsList)
            {
                var result = resultSet.result;
                var instanceType = result.GetType();
                var memberInfos = instanceType.GetMember(plottableValue.PropertyName);
                foreach (var memberInfo in memberInfos)
                {
                    switch (memberInfo.MemberType)
                    {
                        case MemberTypes.Property:
                        {
                            var propertyInfo = memberInfo as PropertyInfo;
                            var propertyValue = Convert.ToDouble(propertyInfo.GetValue(result, null));
                            pointList.Add(new PlotPoint1D { X = resultSet.X, Y = propertyValue });
                            break;
                        }
                        case MemberTypes.Field:
                        {
                            var fieldInfo = memberInfo as FieldInfo;
                            var propertyValue = Convert.ToDouble(fieldInfo.GetValue(result));
                            pointList.Add(new PlotPoint1D { X = resultSet.X, Y = propertyValue });
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
    }
}
