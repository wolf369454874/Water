using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;

namespace Water.Business
{
    /// <summary>
    /// 模型基类
    /// </summary>
    [System.Serializable]
    public abstract class ModelBase
    {
        /// <summary>
        /// 模型基类构造函数
        /// </summary>
        public ModelBase() { }

        /// <summary>
        /// 模型基类构造函数
        /// </summary>
        /// <param name="source">源数据</param>
        public ModelBase(object source)
        {
            this.ModelAdapt(source);
        }

        /// <summary>
        /// 模型基类构造函数
        /// </summary>
        /// <param name="source">源数据</param>
        /// <param name="adapter">数据适配器</param>
        public ModelBase(object source, IModelAdapt adapter)
        {
            this.ModelAdapt(source, adapter);
        }

        /// <summary>
        /// 根据源数据获取模型
        /// </summary>
        /// <param name="source">源数据</param>
        public void ModelAdapt(object source)
        {
            if (source == null)
                return;
            var targetProperties = this.GetType().GetProperties().ToList();
            targetProperties.ForEach(new Action<PropertyInfo>(delegate(PropertyInfo tPI)
            {
                if (tPI.CanWrite)
                {
                    string piName = tPI.Name;
                    var attributes = tPI.GetCustomAttributes(typeof(ModelMap), true);
                    if (tPI.PropertyType.GetInterface("IList") != null)
                    {
                        #region 模型集合映射
                        if (attributes != null && attributes.Count() > 0)
                        {
                            var attr = attributes.Select(att => att as ModelMap).FirstOrDefault(att => att.SourceType == source.GetType());
                            if (attr != null)
                            {
                                var sourceDatas = source.GetType().GetProperty(attr.PropertyName).GetValue(source, null);
                                if (sourceDatas != null && sourceDatas.GetType().GetInterface("IEnumerable") != null)
                                {
                                    var tResult = (IList)Activator.CreateInstance(tPI.PropertyType);
                                    var enumerator = ((IEnumerable)sourceDatas).GetEnumerator();
                                    enumerator.Reset();
                                    while (enumerator.MoveNext())
                                    {
                                        try
                                        {
                                            var m = (ModelBase)Activator.CreateInstance(tPI.PropertyType.GetGenericArguments()[0]);
                                            m.ModelAdapt(enumerator.Current);
                                            tResult.Add(m);
                                        }
                                        catch { }
                                    }
                                    tPI.SetValue(this, tResult, null);
                                }
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        #region 模型映射
                        if (attributes != null && attributes.Count() > 0)
                        {
                            var attr = attributes.Select(att => att as ModelMap).FirstOrDefault(att => att.SourceType == source.GetType());
                            if (attr != null)
                                piName = attr.PropertyName;
                        }
                        var sourcePI = source.GetType().GetProperty(piName);
                        if (sourcePI != null && sourcePI.CanRead)
                            tPI.SetValue(this, sourcePI.GetValue(source, null), null);
                        #endregion
                    }
                }
            }));
        }

        /// <summary>
        /// 模型数据适配
        /// </summary>
        /// <param name="source">源数据</param>
        /// <param name="adapter">数据适配器</param>
        public void ModelAdapt(object source, IModelAdapt adapter)
        {
            if (source == null)
                return;
            adapter.ModelAdapt(this, source);
        }

        /// <summary>
        /// 将模型转换为数据对象
        /// </summary>
        /// <typeparam name="T">数据对象类型</typeparam>
        /// <returns></returns>
        public T ModelToEntity<T>()
        {
            T entity = Activator.CreateInstance<T>();
            var sourceProperties = this.GetType().GetProperties().ToList();
            sourceProperties.ForEach(new Action<PropertyInfo>(delegate(PropertyInfo sourcePI)
            {
                if (sourcePI.CanRead)
                {
                    string piName = sourcePI.Name;
                    if (sourcePI.PropertyType.GetInterface("IList") != null)
                    {
                        #region 模型集合映射
                        var attributes = sourcePI.GetCustomAttributes(typeof(ModelMap), true);
                        if (attributes != null && attributes.Count() > 0)
                        {
                            var attr = attributes.Select(att => att as ModelMap).FirstOrDefault(att => att.SourceType == entity.GetType());
                            if (attr != null)
                            {
                                var sourceDatas = sourcePI.GetValue(this, null);
                                var tPI = entity.GetType().GetProperty(attr.PropertyName);
                                if (sourceDatas != null && tPI != null)
                                {
                                    var tResult = (IList)Activator.CreateInstance(tPI.PropertyType);
                                    var enumerator = ((IEnumerable)sourceDatas).GetEnumerator();
                                    enumerator.Reset();
                                    while (enumerator.MoveNext())
                                    {
                                        try
                                        {
                                            var result = typeof(ModelBase).GetMethod("ModelToEntity", System.Type.EmptyTypes).MakeGenericMethod(tPI.PropertyType.GetGenericArguments()[0]).Invoke(enumerator.Current, null);
                                            tResult.Add(result);
                                        }
                                        catch { }
                                    }
                                    tPI.SetValue(entity, tResult, null);
                                }
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        #region 模型映射
                        var attributes = sourcePI.GetCustomAttributes(typeof(ModelMap), true);
                        if (attributes != null && attributes.Count() > 0)
                        {
                            var attr = attributes.Select(att => att as ModelMap).FirstOrDefault(att => att.SourceType == entity.GetType());
                            if (attr != null)
                                piName = attr.PropertyName;
                        }
                        var tPI = entity.GetType().GetProperty(piName);
                        if (tPI != null && tPI.CanWrite)
                            tPI.SetValue(entity, sourcePI.GetValue(this, null), null);
                        #endregion
                    }
                }
            }));
            return entity;
        }

        /// <summary>
        /// 通过自定义转换接口实现将模型转换为数据对象
        /// </summary>
        /// <typeparam name="T">数据对象类型</typeparam>
        /// <param name="transfer">自定义转换接口实现</param>
        /// <returns></returns>
        public T ModelToEntity<T>(IModelToEntity<T> transfer)
        {
            return transfer.ModelToEntity(this);
        }
    }

    /// <summary>
    /// 模型属性映射关系
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ModelMap : Attribute
    {
        /// <summary>
        /// 元数据类型
        /// </summary>
        public Type SourceType { get; set; }
        /// <summary>
        /// 对应属性名称
        /// </summary>
        public string PropertyName { get; set; }
    }

    /// <summary>
    /// 自定义模型适配接口
    /// </summary>
    public interface IModelAdapt
    {
        /// <summary>
        /// 自定义模型适配方法
        /// </summary>
        /// <param name="model">模型</param>
        /// <param name="source">源数据</param>
        void ModelAdapt(ModelBase model, object source);
    }

    /// <summary>
    /// 自定义模型转换为数据对象接口
    /// </summary>
    /// <typeparam name="T">数据实例类型</typeparam>
    public interface IModelToEntity<T>
    {
        /// <summary>
        /// 将模型转换为数据对象
        /// </summary>
        /// <param name="model">数据对象类型</param>
        /// <returns></returns>
        T ModelToEntity(ModelBase model);
    }

    /// <summary>
    /// 模型方法扩展
    /// </summary>
    public static class ModelExtend
    {
        /// <summary>
        /// 模型集合适配
        /// </summary>
        /// <typeparam name="M">模型类型</typeparam>
        /// <param name="models">模型集合</param>
        /// <param name="soruce">数据源</param>
        public static void ModelsAdapt<M>(this IList<M> models, IEnumerable soruce)
            where M : ModelBase
        {
            if (soruce == null)
                return;
            var enumerator = soruce.GetEnumerator();
            enumerator.Reset();
            while (enumerator.MoveNext())
            {
                var m = Activator.CreateInstance<M>();
                m.ModelAdapt(enumerator.Current);
                models.Add(m);
            }
        }

        /// <summary>
        /// 模型集合适配
        /// </summary>
        /// <typeparam name="M">模型类型</typeparam>
        /// <param name="models">模型集合</param>
        /// <param name="soruce">数据源</param>
        /// <param name="adapter">自定义适配器</param>
        public static void ModelsAdapt<M>(this IList<M> models, IEnumerable soruce, IModelAdapt adapter)
            where M : ModelBase
        {
            if (soruce == null)
                return;
            var enumerator = soruce.GetEnumerator();
            enumerator.Reset();
            while (enumerator.MoveNext())
            {
                var m = Activator.CreateInstance<M>();
                m.ModelAdapt(enumerator.Current, adapter);
                models.Add(m);
            }
        }

        /// <summary>
        /// 模型集合转换
        /// </summary>
        /// <typeparam name="M">模型类型</typeparam>
        /// <typeparam name="T">转换目标类型</typeparam>
        /// <param name="models">模型集合</param>
        /// <returns></returns>
        public static List<T> ModelsToEntitys<M, T>(this IList<M> models)
            where M : ModelBase
        {
            List<T> result = new List<T>();
            if (models.Count == 0)
                return result;
            models.ToList().ForEach(new Action<M>(delegate(M model)
                {
                    result.Add(model.ModelToEntity<T>());
                }));
            return result;
        }

        /// <summary>
        /// 模型集合转换
        /// </summary>
        /// <typeparam name="M">模型类型</typeparam>
        /// <typeparam name="T">转换目标类型</typeparam>
        /// <param name="models">模型集合</param>
        /// <param name="transfer">自定义转换器</param>
        /// <returns></returns>
        public static List<T> ModelsToEntitys<M, T>(this IList<M> models, IModelToEntity<T> transfer)
            where M : ModelBase
        {
            List<T> result = new List<T>();
            if (models.Count == 0)
                return result;
            models.ToList().ForEach(new Action<M>(delegate(M model)
            {
                result.Add(model.ModelToEntity<T>(transfer));
            }));
            return result;
        }
    }
}
