﻿using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraScheduler;
using DevExpress.XtraTreeList.Columns;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace timelineview_datatable
{
    public partial class Form1 : DevExpress.XtraEditors.XtraForm
    {
        DataTable sample_dt;
        DataTable resrc_dt;
        DataTable todo_dt;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //splitContainerControl3.FixedPanel = DevExpress.XtraEditors.SplitFixedPanel.Panel1;
            splitContainerControl3.Panel1.MinSize = 1000;
            MakeSampleData();
            MakeResourceData();
            MakeTodoData();
            InitResources();
            InitScheduler();
            BindResources();
            BindScheduler();
            gridView1.OptionsBehavior.Editable = false;
           
        }

        private void MakeTodoData()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("work_no"); 
            dt.Columns.Add("work_nm");
            dt.Columns.Add("subject");
            dt.Columns.Add("remark");
            dt.Columns.Add("worker");
            dt.Columns.Add("spend_time");

            dt.Rows.Add("W100", "작업1번", "물끓이기", "물양 잘맞춰야함", "득구",TimeSpan.FromDays(2).Days);
            dt.Rows.Add("W101", "작업2번", "스프넣기", "아 매콤하다", "길동", TimeSpan.FromDays(1).Days);
            dt.Rows.Add("W102", "작업3번", "면 넣기", "아 쫄깃하다", "한둑", TimeSpan.FromDays(4).Days);
            dt.Rows.Add("W103", "작업4번", "먹기!", "아 맛있다", "길구", TimeSpan.FromDays(5).Days);

            gridControl1.DataSource = dt;
        }

        private void MakeResourceData()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("machine_no"); //resource
            dt.Columns.Add("machine_nm"); //resource
            dt.Rows.Add("M001", "Machine A");
            dt.Rows.Add("M002", "Machine B");
            dt.Rows.Add("M003", "Machine C");
            resrc_dt = dt;

        }

        private void InitResources()
        {
            resourcesTree1.OptionsView.ShowAutoFilterRow = true;
        }

        /// <summary>
        /// Resources 정보를 바인딩
        /// </summary>
        private void BindResources()
        {
            ISchedulerStorage ds = schedulerControl1.DataStorage;
            ds.Resources.DataSource = resrc_dt;
            ds.Resources.Mappings.Id = "machine_no";
            ds.Resources.Mappings.Caption = "machine_nm";
            AddColumnResourcesTree();
        }

        /// <summary>
        /// ResourcesTree에 표기할 Column 추가
        /// </summary>
        private void AddColumnResourcesTree()
        {
            foreach(DataColumn col in resrc_dt.Columns)
            {
                TreeListColumn treeCol = new TreeListColumn();
                treeCol.Caption = col.Caption;
                treeCol.FieldName = col.ColumnName;
                treeCol.Visible = true;
                resourcesTree1.Columns.Add(treeCol);
            }
        }

        /// <summary>
        /// SechedulerControl에 데이터테이블 바인딩
        /// </summary>
        private void BindScheduler()
        {
            ISchedulerStorage ds = schedulerControl1.DataStorage;
            ds.Appointments.DataSource = sample_dt;
            ds.Appointments.Mappings.Start = "start_date";
            ds.Appointments.Mappings.End = "end_date";
            ds.Appointments.Mappings.Subject = "Subject";
            ds.Appointments.Mappings.Description = "remark";
            ds.Appointments.CustomFieldMappings.Add(new AppointmentCustomFieldMapping("worker", "worker"));
            ds.Appointments.CustomFieldMappings.Add(new AppointmentCustomFieldMapping("work_no", "work_no"));

            ds.Appointments.Mappings.ResourceId = "machine_no";
        }

        /// <summary>
        /// Appointment를 그룹화 하기위한 Resources 데이터 생성 (스케줄 데이터 기반으로 그룹바이)
        /// </summary>
        private void MakeResourceDataBasedOnScheduleData()
        {
            //스케줄 데이터 기반으로 그룹바이 하기때문에, 스케줄데이터에 없는 정보는 표기되지 않게됨.
            DataTable dt = new DataTable();
            dt.Columns.Add("machine_no");
            dt.Columns.Add("machine_nm");
            dt = sample_dt.AsEnumerable().
                                    GroupBy(r => new
                                    {
                                        machine_no = r["machine_no"],
                                        machine_nm = r["machine_nm"]
                                    }).Select(g =>
                                    {
                                        DataRow row = dt.NewRow();
                                        row["machine_no"] = g.Key.machine_no;
                                        row["machine_nm"] = g.Key.machine_nm;
                                        return row;
                                    }).CopyToDataTable();
            dt.Columns["machine_no"].Caption = "설비코드";
            dt.Columns["machine_nm"].Caption = "설비명";
            resrc_dt = dt;
            
        }

        /// <summary>
        /// 테스트용 샘플데이터
        /// </summary>
        private void MakeSampleData()
        {
            DataTable apt_dt = new DataTable();
            //apt_dt.Columns.Add("id"); //resource
            apt_dt.Columns.Add("machine_no"); //resource
            apt_dt.Columns.Add("machine_nm"); //resource
            apt_dt.Columns.Add("work_no"); //id,resource
            apt_dt.Columns.Add("work_nm");
            apt_dt.Columns.Add("start_date", typeof(DateTime));
            apt_dt.Columns.Add("end_date", typeof(DateTime));
            apt_dt.Columns.Add("subject");
            apt_dt.Columns.Add("remark");
            apt_dt.Columns.Add("worker");
            // apt_dt.Columns.Add("ResourceID");

            // 샘플 데이터 추가
            //apt_dt.Rows.Add("M001", "Machine A", "W001", "Work Alpha", Convert.toDateTime("2024-01-01"), DateTime.Now.AddHours(2), "Task A", "Remark 1", "Worker 1");
            apt_dt.Rows.Add("M001", "Machine A", "W001", "Work Alpha", DateTime.Now, DateTime.Now.AddHours(2), "Task A", "Remark 1", "Worker 1");
            apt_dt.Rows.Add("M002", "Machine B", "W002", "Work Beta", DateTime.Now.AddHours(1), DateTime.Now.AddHours(3), "Task B", "Remark 2", "Worker 2");
            apt_dt.Rows.Add("M003", "Machine C", "W003", "Work Gamma", DateTime.Now.AddHours(2), DateTime.Now.AddHours(4), "Task C", "Remark 3", "Worker 3");
            apt_dt.Rows.Add("M001", "Machine A", "W004", "Work Delta", DateTime.Now.AddHours(3), DateTime.Now.AddHours(5), "Task D", "Remark 4", "Worker 1");
            apt_dt.Rows.Add("M002", "Machine B", "W005", "Work Epsilon", DateTime.Now.AddHours(4), DateTime.Now.AddHours(6), "Task E", "Remark 5", "Worker 2");
            apt_dt.Rows.Add("M003", "Machine C", "W006", "Work Zeta", DateTime.Now.AddHours(5), DateTime.Now.AddHours(7), "Task F", "Remark 6", "Worker 3");
            apt_dt.Rows.Add("M001", "Machine A", "W007", "Work Eta", DateTime.Now.AddHours(6), DateTime.Now.AddHours(8), "Task G", "Remark 7", "Worker 1");
            apt_dt.Rows.Add("M002", "Machine B", "W008", "Work Theta", DateTime.Now.AddHours(7), DateTime.Now.AddHours(9), "Task H", "Remark 8", "Worker 2");
            apt_dt.Rows.Add("M003", "Machine C", "W009", "Work Iota", DateTime.Now.AddHours(8), DateTime.Now.AddHours(10), "Task I", "Remark 9", "Worker 3");
            apt_dt.Rows.Add("M001", "Machine A", "W010", "Work Kappa", DateTime.Now.AddHours(9), DateTime.Now.AddHours(11), "Task J", "Remark 10", "Worker 1");
            sample_dt = apt_dt;
            gridControl2.DataSource = sample_dt;

        }

        /// <summary>
        /// SchedulerControl 속성초기화
        /// </summary>
        private void InitScheduler()
        {
            schedulerControl1.ActiveViewType = DevExpress.XtraScheduler.SchedulerViewType.Timeline;
            schedulerControl1.GroupType = SchedulerGroupType.Resource;

        }


        #region gridcontrol DragDrop
        GridHitInfo DownHitInfo { get; set; }

        void GridViewTasksMouseDown(object sender, MouseEventArgs e)
        {
            GridView view = sender as GridView;
            this.DownHitInfo = null;

            GridHitInfo hitInfo = view.CalcHitInfo(new Point(e.X, e.Y));
            if (Control.ModifierKeys != Keys.None)
                return;
            if (e.Button == MouseButtons.Left && hitInfo.InRow && hitInfo.HitTest != GridHitTest.RowIndicator)
                this.DownHitInfo = hitInfo;
        }

        void GridViewTasksMouseMove(object sender, MouseEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Button == MouseButtons.Left && this.DownHitInfo != null)
            {
                Size dragSize = SystemInformation.DragSize;
                Rectangle dragRect = new Rectangle(new Point(this.DownHitInfo.HitPoint.X - dragSize.Width / 2,
                    this.DownHitInfo.HitPoint.Y - dragSize.Height / 2), dragSize);

                if (!dragRect.Contains(new Point(e.X, e.Y)))
                {
                    view.GridControl.DoDragDrop(GetDragData(view), DragDropEffects.All);
                    this.DownHitInfo = null;
                }
            }
        }

        void SchedulerControlAppointmentDrop(object sender, AppointmentDragEventArgs e)
        {
            string createEventMsg = "Creating an event at {0} on {1}.";
            string moveEventMsg = "Moving the event from {0} on {1} to {2} on {3}.";

            DateTime srcStart = e.SourceAppointment.Start;
            DateTime newStart = e.EditedAppointment.Start;

            string msg = (srcStart == DateTime.MinValue) ? String.Format(createEventMsg, newStart.ToShortTimeString(), newStart.ToShortDateString()) :
                String.Format(moveEventMsg, srcStart.ToShortTimeString(), srcStart.ToShortDateString(), newStart.ToShortTimeString(), newStart.ToShortDateString());

            if (XtraMessageBox.Show(msg + "\r\nProceed?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                e.Allow = false;
            }
        }

        SchedulerDragData GetDragData(GridView view)
        {
            int[] selection = view.GetSelectedRows();
            if (selection == null)
                return null;

            AppointmentBaseCollection appointments = new AppointmentBaseCollection();
            int count = selection.Length;
            for (int i = 0; i < count; i++)
            {
                int rowIndex = selection[i];
                Appointment apt = schedulerControl1.DataStorage.CreateAppointment(AppointmentType.Normal);
                apt.Subject = (string)view.GetRowCellValue(rowIndex, "subject");
                //apt.ResourceId = (string)view.GetRowCellValue(rowIndex, "MAC_CD");
                //apt.LabelKey = (int)view.GetRowCellValue(rowIndex, "Severity");
                //apt.StatusKey = (int)view.GetRowCellValue(rowIndex, "Priority");
                apt.Start = DateTime.Now;
                //apt.Duration = TimeSpan.FromDays( Convert.ToDouble(view.GetRowCellValue(rowIndex, "spend_time")));
                apt.Duration = TimeSpan.FromDays(Convert.ToDouble(view.GetRowCellValue(rowIndex, "spend_time")));
                //apt.Duration = TimeSpan.FromHours((int)view.GetRowCellValue(rowIndex, "Duration"));
                apt.Description = (string)view.GetRowCellValue(rowIndex, "remark");
                apt.CustomFields["worker"] = (string)view.GetRowCellValue(rowIndex, "worker");
                apt.CustomFields["work_no"] = (string)view.GetRowCellValue(rowIndex, "work_no");
                appointments.Add(apt);

            }

            return new SchedulerDragData(appointments, 0);
        }
        #endregion gridcontrol DragDrop

    }
}
