using System;
using System.Linq;
using System.Reflection;
using WGX.Common.Attributes;
using System.Linq.Dynamic;

namespace WGX.Common {

    [Serializable]
    public enum OrderDirection {
        Asc,
        Desc
    }

    [Serializable]
    public class BaseQuery {

        private Pager _pager;
        public Pager Pager {
            get {
                if (_pager == null)
                    _pager = new Pager();
                return _pager;
            }
            set {
                _pager = value;
            }
        }

        private bool _allowPage = true;
        /// <summary>
        /// 是否分页
        /// </summary>
        [DisableBinding]
        public bool AllowPage {
            get {
                return _allowPage;
            }
            set {
                _allowPage = value;
            }
        }

        //public IQueryable Filter(Func<IQueryable> func) {
        //    var source = func.Invoke();

        //    return source;
        //}

        //protected virtual IQueryable DoPage(IQueryable source) {
        //    this.Pager.Count = source.Count();

        //    if (this.AllowPage) {
        //        return source
        //            .Skip(this.Pager.Page * this.Pager.PageSize)
        //            .Take(this.Pager.PageSize).AsQueryable();
        //    } else
        //        return source;
        //}
    }

    [Serializable]
    public class BaseQuery<T> : BaseQuery where T : class {

        //private Dictionary<Func<T, object>, OrderDirection> orderBys = null;

        //[DisableBinding]
        //public Dictionary<Func<T, object>, OrderDirection> OrderBys {
        //    get {
        //        if (this.orderBys == null)
        //            this.orderBys = new Dictionary<Func<T, object>, OrderDirection>();
        //        return this.orderBys;
        //    }
        //    set {
        //        this.orderBys = value;
        //    }
        //}


        public IQueryable<T> Filter(IQueryable<T> source) {
            var ps = GetType()
                .GetProperties()
                .ToList();

            ps.ForEach(p => {
                var mapTo = p.GetCustomAttribute<MapToAttribute>();
                if (mapTo != null) {
                    var st = typeof(T).GetProperty(mapTo.Field);
                    if (st != null) {
                        var opt = string.Empty;
                        switch (mapTo.Opt) {
                            case MapToOpts.Equal:
                                opt = "==";
                                break;
                            case MapToOpts.NotEqual:
                                opt = "!=";
                                break;
                            case MapToOpts.Gt:
                                opt = ">";
                                break;
                            case MapToOpts.Lt:
                                opt = "<";
                                break;
                            case MapToOpts.GtOrEqual:
                                opt = ">=";
                                break;
                            case MapToOpts.LtOrEqual:
                                opt = "<=";
                                break;
                        }

                        if (!string.IsNullOrEmpty(opt)) {
                            var v = p.GetValue(this);
                            if (v != null) {
                                string cond;
                                if (v is string && mapTo.Opt == MapToOpts.Equal && mapTo.IgnoreCase) {
                                    if (!string.IsNullOrWhiteSpace(v.ToString()))
                                    {
                                        cond = string.Format("{0}.ToUpper() {1} @0", st.Name, opt);
                                        source = source.Where(cond, ((string)v).ToUpper());
                                    }
                                } else {
                                    cond = string.Format("{0} {1} @0", st.Name, opt);
                                    source = source.Where(cond, v);
                                }


                            }
                        } else {
                            var cond = string.Empty;

                            var v = (string)p.GetValue(this);
                            if (v == null || string.IsNullOrEmpty(v))
                                return;

                            v = v.Replace("\"", "");

                            if (string.IsNullOrEmpty(v))
                                return;

                            var ignoreCaseStr = mapTo.IgnoreCase ? ".ToUpper()" : "";
                            if (mapTo.IgnoreCase)
                                v = v.ToUpper();

                            switch (mapTo.Opt) {
                                case MapToOpts.Include:
                                    cond = string.Format("{0}{1}.IndexOf(\"{2}\") != -1", st.Name, ignoreCaseStr, v);
                                    break;
                                case MapToOpts.StartWith:
                                    cond = string.Format("{0}{1}.StartsWith(\"{2}\")", st.Name, ignoreCaseStr, v);
                                    break;
                                case MapToOpts.EndWith:
                                    cond = string.Format("{0}{1}.EndsWith(\"{2}\")", st.Name, ignoreCaseStr, v);
                                    break;
                            }

                            if (!string.IsNullOrEmpty(cond)) {
                                source = source.Where(cond);
                            }

                        }
                    }
                }
            });

            return source;
        }
    }
}
