using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Boxup.Entities;
using Boxup.Data;
namespace Boxup
{
    public partial class FormItem : Form
    {
        #region Property
        protected List<ItemInfo> Items
        {
            get;
            set;
        }
        #endregion
        public FormItem()
        {
            InitializeComponent();
            this.Items = DataRepository.ItemProvider.GetAll(null) as List<ItemInfo>;

            foreach (var obj in this.Items)
            {
                this.lbItem.Items.Add(string.Format("{0}|{1}",obj.ItemName,obj.d));
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtItem.Text))
            {
                MessageBox.Show("请先输入物料的特征码!");
            }
            else
            {
                var ss = this.txtItem.Text.Split("\r\n".ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
                foreach (var s in ss)
                {
                    var sa  = s.Split("\t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    if (this.Items.Exists(item => item.ItemName.Equals(sa[0].Trim(), StringComparison.OrdinalIgnoreCase)))
                    {
//                        MessageBox.Show("输入的物料特征码已经存在了！");
                    }
                    else
                    {
                        var obj = new ItemInfo();
                        obj.ItemName = sa[0].Trim();
                        obj.OldItemName = obj.ItemName;
                        if(sa.Length>1)
                        {
                            obj.d = int.Parse(sa[1]);
                        }
                        else
                        {
                            obj.d = 0;
                        }
                        if (DataRepository.ItemProvider.Insert(obj, null))
                        {
                            this.Items.Add(obj);
                            this.lbItem.Items.Add(string.Format("{0}|{1}", obj.ItemName, obj.d));
                        }
                    }
                }
               
                
                
            }
            

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (this.lbItem.SelectedItem != null)
            {
                
                if (DataRepository.ItemProvider.Delete(this.lbItem.SelectedItem.ToString().Split("|".ToCharArray())[0],null) == false)
                {
                    MessageBox.Show("删除失败！");
                }
                else 
                {
                    var item = this.Items.Find(o => o.ItemName == this.lbItem.SelectedItem.ToString().Split("|".ToCharArray())[0]);
                    this.Items.Remove(item);
                    this.lbItem.Items.Remove(string.Format("{0}|{1}",item.ItemName,item.d));
                }
            }
            else
            {
                MessageBox.Show("请先选择一条记录再进行删除！");
            }
        }
    }
}
