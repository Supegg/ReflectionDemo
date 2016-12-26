using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;

namespace DynamicDemoApp
{
    public class CustomExpando : DynamicObject
    {
        public IDictionary<string, object> Dictionary { get; set; }

        public CustomExpando()
        {
            this.Dictionary = new Dictionary<string, object>();
        }

        public int Count { get { return this.Dictionary.Keys.Count; } }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (this.Dictionary.ContainsKey(binder.Name))
            {
                result = this.Dictionary[binder.Name];
                return true;
            }
            return base.TryGetMember(binder, out result); //means result = null and return = false
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (!this.Dictionary.ContainsKey(binder.Name))
            {
                this.Dictionary.Add(binder.Name, value);
            }
            else
                this.Dictionary[binder.Name] = value;

            return true;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            if (this.Dictionary.ContainsKey(binder.Name) && this.Dictionary[binder.Name] is Delegate)
            {
                Delegate del = this.Dictionary[binder.Name] as Delegate;
                result = del.DynamicInvoke(args);
                return true;
            }
            return base.TryInvokeMember(binder, args, out result);
        }

        public override bool TryDeleteMember(DeleteMemberBinder binder)
        {
            if (this.Dictionary.ContainsKey(binder.Name))
            {
                this.Dictionary.Remove(binder.Name);
                return true;
            }

            return base.TryDeleteMember(binder);
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            foreach (string name in this.Dictionary.Keys)
                yield return name;
        }
    }
}
