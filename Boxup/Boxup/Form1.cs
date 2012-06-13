using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Boxup.Data;
using Boxup.Entities;

using Microsoft.Reporting.WinForms;

namespace Boxup
{
    public partial class Form1 : Form
    {
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Property
        public string LastPath { get; set; }
        public DateTime CurrentDate { get { return this.dtpBoxDate.Value.Date; } }
        #endregion

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            #region clear oldData
            using (var trans = DataRepository.Provider.CreateTransaction())
            {
                trans.BeginTransaction();
                var boxs = DataRepository.BoxProvider.GetByBoxDate(this.CurrentDate, trans);
                if (boxs.Count > 0)
                {
                    if (MessageBox.Show("指定日期已经存在记录，是否覆盖原有的记录！", "确认", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                            foreach (var obj in boxs)
                            {
                                if (DataRepository.BoxProvider.Delete(obj, trans) == false)
                                {
                                    trans.Rollback();
                                    return;
                                }
                            }
                            var boxContracts = DataRepository.BoxContractProvider.GetByBoxDate(this.CurrentDate, trans);
                            foreach (var obj in boxContracts)
                            {
                                if (DataRepository.BoxContractProvider.Delete(obj, trans) == false)
                                {
                                    trans.Rollback();
                                    return;
                                }
                            }
                            var boxItems = DataRepository.BoxItemProvider.GetByBoxDate(this.CurrentDate, trans);
                            foreach (var obj in boxItems)
                            {
                                if (DataRepository.BoxItemProvider.Delete(obj, trans) == false)
                                {
                                    trans.Rollback();
                                    return;
                                }
                            }
                        }
                    }
                trans.Commit();
            }
            #endregion

            var path = DataRepository.PathProvider.Get();
            if (path != null)
            {
                this.LastPath = path.LastPath;
                this.openFileDialog1.InitialDirectory = path.LastPath;
            }
            this.openFileDialog1.Filter = "文本文件(*.txt)|*.txt";

            if(this.openFileDialog1.ShowDialog()==DialogResult.OK)
            {
                var fileName = this.openFileDialog1.FileName;
                var currentPath = Directory.GetParent(fileName).FullName;
                if (!string.IsNullOrEmpty(this.LastPath))
                {
                    if (this.LastPath != currentPath)
                    {
                        this.LastPath = currentPath;
                        DataRepository.PathProvider.Delete();
                        DataRepository.PathProvider.Insert(new PathInfo { LastPath = this.LastPath });
                    }
                }
                StreamReader sr = new StreamReader(this.openFileDialog1.FileName, System.Text.Encoding.Default);
                var linenumber = 0;
                var line = sr.ReadLine().Trim();
                //var IsTaskRegion = false;
                if (line.Contains("任务清单"))
                {
                    //IsTaskRegion = true;
                }
                line = sr.ReadLine().Trim();
                if (string.IsNullOrEmpty(line))
                    line = sr.ReadLine().Trim();

                using (var trans = DataRepository.Provider.CreateTransaction())
                {
                    trans.BeginTransaction();
                    while (!string.IsNullOrEmpty(line))
                    {
                        //Task 

                        var tasks = line.Split("\t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        var obj = new BoxContractInfo { BoxNo = int.Parse(tasks[0].Trim()), ContractNo = tasks[1].Trim(), BoxDate = this.CurrentDate };
                        DataRepository.BoxContractProvider.Insert(obj, trans);
                        line = sr.ReadLine().Trim();
                        linenumber++;
                    }
                    //IsTaskRegion = false;

                    var items = DataRepository.ItemProvider.GetAll(trans) as List<ItemInfo>;

                    BoxItemInfo mainBoxItemInfo = null;
                    line = sr.ReadLine().Trim();
                    while (line != null)//End of File.
                    {
                        if (!string.IsNullOrEmpty(line) && !line.Contains("装箱日期：") && !line.Contains("打印日期"))
                        {
                            if (!line.Contains("<="))
                            {
                                var boxItems = line.Split("\t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                                if (boxItems.Length == 5)
                                {
                                    string model = "";
                                    if (boxItems[1].LastIndexOf('L') < 0)
                                        model = boxItems[1];
                                    else 
                                        model = boxItems[1].Substring(0,boxItems[1].LastIndexOf('L'));

                                    //if (items.Find(item => item.ItemName == boxItems[2].Trim()) != null)
                                    if(items.Find(item=>item.ItemName.Equals(model,StringComparison.OrdinalIgnoreCase))!=null)
                                        mainBoxItemInfo = new BoxItemInfo { ItemModel = boxItems[1].Trim(), ItemName = boxItems[2].Trim() };
                                    else if (boxItems[2].Contains("24#箱~"))
                                    {
                                        mainBoxItemInfo = new BoxItemInfo { ItemModel = boxItems[1].Trim(), ItemName = boxItems[2].Trim() };
                                    }
                                    else
                                        mainBoxItemInfo = null;
                                }
                            }
                            else
                            {
                                if (mainBoxItemInfo != null)
                                {
                                    var detailItems = line.Split("\t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                                    if (detailItems.Length % 3 == 0)
                                    {
                                        if (mainBoxItemInfo.ItemName.Contains("24#箱~"))
                                        {
                                            for (var i = 0; i < detailItems.Length; i = i + 3)
                                            {
                                                var boxInfo = new BoxInfo();
                                                boxInfo.BoxModel = mainBoxItemInfo.ItemModel;
                                                boxInfo.BoxName = mainBoxItemInfo.ItemName.Split("~".ToCharArray())[0];
                                                boxInfo.BoxSpec = mainBoxItemInfo.ItemName.Split("~".ToCharArray())[1];
                                                boxInfo.BoxNo = int.Parse(detailItems[i].Trim());
                                                boxInfo.BoxDate = this.CurrentDate;
                                                if (DataRepository.BoxProvider.Insert(boxInfo, trans) == 0)
                                                {
                                                    trans.Rollback();
                                                    return;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            for (var i = 0; i < detailItems.Length; i = i + 3)
                                            {
                                                var boxItemInfo = new BoxItemInfo();
                                                boxItemInfo.ItemModel = mainBoxItemInfo.ItemModel;
                                                boxItemInfo.ItemName = mainBoxItemInfo.ItemName;
                                                boxItemInfo.BoxNo = int.Parse(detailItems[i].Trim());
                                                boxItemInfo.ItemNum = int.Parse(detailItems[i + 2].Trim());
                                                boxItemInfo.BoxDate = this.CurrentDate;
                                                if (DataRepository.BoxItemProvider.Insert(boxItemInfo, trans) == 0)
                                                {
                                                    trans.Rollback();
                                                    return;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        line = sr.ReadLine();
                    }

                    trans.Commit();

                    MessageBox.Show("解析成功！");

                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           
        }

        private void btnShowReport_Click(object sender, EventArgs e)
        {
            var boxDate = this.dtpBoxDate.Value.Date;
            this.reportViewer1.LocalReport.SetParameters(new[] { new ReportParameter("BoxDate", boxDate.ToShortDateString()) });
            this.DataTable1TableAdapter.Fill(this.BoxDataSet.DataTable1, boxDate);
            this.reportViewer1.RefreshReport();
            

            this.reportViewer2.LocalReport.SetParameters(new[] { new ReportParameter("BoxDate", boxDate.ToShortDateString()) });
            this.DataTable2TableAdapter.Fill(this.BoxDataSet.DataTable2,boxDate);
            this.reportViewer2.RefreshReport();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            new FormItem().Show();
        }
    }
}
