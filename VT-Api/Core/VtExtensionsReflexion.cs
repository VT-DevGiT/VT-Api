using System;
using System.Reflection;

namespace VT_Api.Reflexion
{
    public static class VtExtensionsReflexion
    {
        public static object CallMethod(this Type o, string methodName, params object[] args)
        {
            var mi = o.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            if (mi != null)
            {
                return mi.Invoke(null, args);
            }
            return null;
        }

        public static object CallMethod(this object o, string methodName, params object[] args)
        {
            var mi = o.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (mi != null)
            {
                return mi.Invoke(o, args);
            }
            return null;
        }

        public static T GetFieldValueOrPerties<T>(this object element, string fieldName)
        {
            var prop = element.GetType().GetProperty(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (prop != null)
            {
                return (T)prop.GetValue(element);
            }
            FieldInfo field = element.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
            {
                return (T)field.GetValue(element);
            }
            return default(T);
        }

        public static T GetFieldOrPropertyValue<T>(this Type element, string fieldName)
        {
            var prop = element.GetProperty(fieldName, BindingFlags.NonPublic | BindingFlags.Static);
            if (prop != null)
            {
                return (T)prop.GetValue(null);
            }
            var field = element.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static);
            if (field != null)
            {
                return (T)field.GetValue(null);
            }
            return default(T);
        }

        public static void SetProperty<T>(this Type element, string fieldName, T value)
        {
            var prop = element.GetType().GetProperty(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            if (prop != null)
            {
                prop.SetValue(null, value);
            }
        }

        public static void SetProperty<T>(this object element, string fieldName, T value)
        {
            var prop = element.GetType().GetProperty(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (prop != null)
            {
                prop.SetValue(element, value);
            }
        }

        public static void SetField<T>(this object element, string fieldName, T value)
        {
            var prop = element.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (prop != null)
            {
                prop.SetValue(element, value);
            }
        }

        public static void SetField<T>(this Type element, string fieldName, T value)
        {
            var prop = element.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static);
            if (prop != null)
            {
                prop.SetValue(null, value);
            }
        }

        public static void CallEvent(this Type o, string eventName,params object[] parameters)
        {
            var eventsField = o.GetField(eventName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            if (eventsField != null)
            {
                object eventHandlerList = eventsField.GetValue(null);
                if (eventHandlerList != null)
                {
                    var my_event_invoke = eventHandlerList.GetType().GetMethod("Invoke");
                    if (my_event_invoke != null)
                    {
                        my_event_invoke.Invoke(eventHandlerList, parameters);
                    }
                }
                else Synapse.Api.Logger.Get.Error("Vt-Reflexion: CallEvent failed!! \n eventHandlerList null");
            }
            else
            {
                Synapse.Api.Logger.Get.Error("Vt-Reflexion: CallEvent failed!! \n eventsField null");
            }
        }

        public static void CallEvent(this object element, string eventName, params object[] parameters)
        {
            var eventsField = element.GetType().GetField(eventName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (eventsField != null)
            {
                try
                {
                    object eventHandlerList = eventsField.GetValue(element);
                    if (eventHandlerList != null)
                    {
                        var my_event_invoke = eventHandlerList.GetType().GetMethod("Invoke");
                        if (my_event_invoke != null)
                        {
                            my_event_invoke.Invoke(eventHandlerList, parameters);
                        }
                    }
                }
                catch (Exception e)
                {
                    Synapse.Api.Logger.Get.Error($"Vt-Reflexion: CallEvent {eventName} failed!!\n{e}\nStackTrace:\n{e.StackTrace}");
                }
                //else Synapse.Api.Logger.Get.Error("Vt-Reflexion: CallEvent failed!! \n eventHandlerList null");
            }
            else
            {
                Synapse.Api.Logger.Get.Error("Vt-Reflexion: CallEvent failed!! \n eventsField null");
            }
        }

        public static T CopyPropertyAndFeild<T>(this T element, T elementToCopy)
        {   
            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (props.Length != 0)
            {
                foreach (var prop in props)
                {
                    if (prop.SetMethod != null)
                        prop.SetValue(element, prop.GetValue(elementToCopy));
                }
            }
            var fields = typeof(T).GetFields(BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            if (fields.Length != 0)
            {
                foreach (var field in fields)
                {
                    field.SetValue(element, field.GetValue(elementToCopy));
                }
            }
            return element;
        }
    }
}
