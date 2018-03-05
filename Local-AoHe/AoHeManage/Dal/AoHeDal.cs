﻿using AoHeManage.Common;
using AoHeManage.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace AoHeManage.Dal
{
    public class AoHeDal
    {
        //public DataSet GetHotelList(string connectionString, int currentPage, int pageSize, string strWhere, string filedOrder)
        //{

        //    StringBuilder strSql = new StringBuilder();
        //    strSql.Append(" select d.ParamName as OperateMode,a.* from (");
        //    strSql.Append(" select  e.*,b.ParamName as Brand  ");
        //    strSql.Append(" from Hotel  e left join dbo.ParamOption b  ");
        //    strSql.Append(" on e.BrandCode = b.ParamOptionCode where b.ParamTypeCode='Brand' ) a ");
        //    strSql.Append(" left join ParamOption d ");
        //    strSql.Append(" on a.OperateModeCode = d.ParamOptionCode  ");
        //    strSql.Append(" where d.ParamTypeCode='OperateMode' ");
        //    if (strWhere.Trim() != "")
        //    {
        //        strSql.Append(strWhere);
        //    }
        //    if (filedOrder.Trim() != "")
        //    {
        //        strSql.Append(" order by " + filedOrder);
        //    }
        //    SqlParameter[] parameters = {
        //            new SqlParameter("@sqlstr", SqlDbType.NVarChar,3500),
        //            new SqlParameter("@currentpage", SqlDbType.Int),
        //            new SqlParameter("@pagesize", SqlDbType.Int)
        //            };
        //    parameters[0].Value = strSql.ToString();
        //    parameters[1].Value = currentPage;
        //    parameters[2].Value = pageSize;
        //    return DbHelperSQL.ExecuteDatasetPageShow(connectionString, parameters);
        //}

        #region 意外事件
        public DataSet GetAccidentInfoList(int currentPage, int pageSize, string strWhere, string filedOrder)
        {
            //select * from table limit (start-1)*limit,limit; 其中start是页码，limit是每页显示的条数。
            StringBuilder strSql = new StringBuilder();
            //strSql.AppendLine(" set @totalrow=0; ");
            //strSql.AppendLine(" set @totalpage=0; ");
            //strSql.AppendLine(" select COUNT(1) into @totalrow from accidentinfo; ");
            //strSql.AppendLine(" set @totalpage=ceiling(1.0*@totalrow/@pagesize); ");
            //strSql.AppendLine(" select @totalrow as totalrow,@totalpage as totalpage; ");
            strSql.AppendLine("  select COUNT(1) as totalrow from accidentinfo;  ");
            strSql.AppendLine(" select a.AccidentID,b.RoomNo,b.BedNo,b.Name,b.Sex,b.Age,b.NurseLevel,a.CreateOn,a.AccidentType,a.Place,a.Condition,a.Remark  ");
            strSql.AppendLine(" from accidentinfo a ");
            strSql.AppendLine(" left join guestinfo b on a.GuestID = b.ID ");
            strSql.AppendFormat(" where 1=1 {0} ", strWhere);
            if (filedOrder.Trim() != "")
            {
                strSql.Append(" order by " + filedOrder);
            }
            strSql.AppendFormat(" LIMIT {0},{1} ", (currentPage - 1) * pageSize, pageSize);

            MySqlParameter[] parameters = {
                        new MySqlParameter("@currentpage", MySqlDbType.Int16),
                        new MySqlParameter("@pagesize", MySqlDbType.Int16)
                        };
            parameters[0].Value = currentPage;
            parameters[1].Value = pageSize;

            return DbHelperSQL.Query(strSql.ToString(), parameters);
        }

        public int AddAccident(Accident model)
        {
            List<String> sqlList = new List<string>();
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine(" insert into accidentinfo (AccidentID,GuestID,CreateOn,AccidentType,Place,`Condition`,Remark) values ");
            strSql.AppendFormat(" (null,'{0}','{1}','{2}','{3}','{4}','{5}' ) ", model.GuestID, model.CreateOn, model.AccidentType, model.Place, model.Condition, model.Remark);
            sqlList.Add(strSql.ToString());
            strSql = new StringBuilder();
            if (model.ListAccidentRelatedPerson != null && model.ListAccidentRelatedPerson.Count > 0)
            {
                foreach (var item in model.ListAccidentRelatedPerson)
                {
                    strSql.AppendLine(" insert into accidentrelatedperson (ID,AccidentID,StaffNo,StaffName) values ");
                    strSql.AppendFormat(" (null,(select a.ID from (select MAX(AccidentID) as ID from accidentinfo) a),'{0}','{1}' ) ; ", item.StaffNo, item.StaffName);
                }
            }
            sqlList.Add(strSql.ToString());
            return DbHelperSQL.ExecuteSqlTran(sqlList);
        }

        public DataSet GetAccidentFollowList(int currentPage, int pageSize, string strWhere, string filedOrder)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine("  select COUNT(1) as totalrow from accidentfollow;  ");
            strSql.AppendLine(" select * from accidentfollow ");
            strSql.AppendFormat(" where 1=1 {0} ", strWhere);
            if (filedOrder.Trim() != "")
            {
                strSql.Append(" order by " + filedOrder);
            }
            strSql.AppendFormat(" LIMIT {0},{1} ", (currentPage - 1) * pageSize, pageSize);
            MySqlParameter[] parameters = {
                        new MySqlParameter("@currentpage", MySqlDbType.Int16),
                        new MySqlParameter("@pagesize", MySqlDbType.Int16)
                        };
            parameters[0].Value = currentPage;
            parameters[1].Value = pageSize;
            return DbHelperSQL.Query(strSql.ToString(), parameters);
        }

        public int AddAccidentFollow(AccidentFollow model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine(" insert into accidentfollow(AccidentID,FollowTime,Remark) values ");
            strSql.AppendFormat(" ('{0}','{1}','{2}' ) ", model.AccidentID, model.FollowTime, model.Remark);
            return DbHelperSQL.ExecuteSql(strSql.ToString());
        }

        public DataSet GetAccidentInfoByWhere(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine(" select a.AccidentID,b.RoomNo,b.BedNo,b.Name,b.Sex,b.Age,b.NurseLevel,a.CreateOn,a.AccidentType,a.Place,a.Condition,a.Remark,d.StaffNo,d.StaffName  ");
            strSql.AppendLine(" from accidentinfo a ");
            strSql.AppendLine(" left join guestinfo b on a.GuestID = b.ID ");
            strSql.AppendLine(" left join accidentrelatedperson d on a.AccidentID = d.AccidentID ");
            strSql.AppendFormat(" where 1=1 {0} ", strWhere);
            return DbHelperSQL.Query(strSql.ToString());
        }

        public Accident GetAccidentModelByID(string accidentID)
        {
            Accident model = new Accident();
            StringBuilder strSql = new StringBuilder();
            DataSet ds = new DataSet();
            strSql.AppendLine(" select a.AccidentID,a.GuestID,a.AccidentType,b.RoomNo,b.BedNo,b.Name,b.Sex,b.Age,b.NurseLevel,a.CreateOn,a.AccidentType,a.Place,a.Condition,a.Remark,d.StaffNo,d.StaffName  ");
            strSql.AppendLine(" from accidentinfo a ");
            strSql.AppendLine(" left join guestinfo b on a.GuestID = b.ID ");
            strSql.AppendLine(" left join accidentrelatedperson d on a.AccidentID = d.AccidentID ");
            strSql.AppendFormat(" where a.AccidentID = '{0}' ", accidentID);
            ds = DbHelperSQL.Query(strSql.ToString());
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var dt = ds.Tables[0];
                model.AccidentID = Convert.ToInt16(dt.Rows[0]["AccidentID"]);
                model.GuestID = Convert.ToInt16(dt.Rows[0]["GuestID"]);
                model.CreateOn = Convert.ToDateTime(dt.Rows[0]["CreateOn"]);
                model.AccidentType = dt.Rows[0]["AccidentType"].ToString();
                model.Place = dt.Rows[0]["Place"].ToString();
                model.Condition = dt.Rows[0]["Condition"].ToString();
                model.Remark = dt.Rows[0]["Remark"].ToString();
                model.RoomNo = dt.Rows[0]["RoomNo"].ToString();
                model.BedNo = dt.Rows[0]["BedNo"].ToString();
                model.Name = dt.Rows[0]["Name"].ToString();
                model.Sex = Convert.ToInt16(dt.Rows[0]["Sex"]);
                model.Age = Convert.ToInt16(dt.Rows[0]["Age"]);
                model.NurseLevel = dt.Rows[0]["NurseLevel"].ToString();
                //相关责任人
                List<AccidentRelatedPerson> listPerson = new List<AccidentRelatedPerson>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var staffNo = dt.Rows[i]["StaffNo"].ToString();
                    var staffName = dt.Rows[i]["StaffName"].ToString();
                    if (!string.IsNullOrWhiteSpace(staffNo))
                    {
                        listPerson.Add(new AccidentRelatedPerson()
                        {
                            StaffNo = staffNo,
                            StaffName = staffName
                        });
                    }
                }
                model.ListAccidentRelatedPerson = listPerson;
            }
            return model;
        }

        public DataSet GetAccidentStats(int currentPage, int pageSize, string strWhere, string filedOrder)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine(" select count(1) as totalrow from ( ");
            strSql.AppendLine(" select c.`Name`,c.Age,c.Sex,c.RoomNo,c.BedNo,a.AccidentType,count(1) as OccurCount from accidentinfo a ");
            strSql.AppendLine(" inner join guestinfo c on a.GuestID = c.ID ");
            strSql.AppendLine(" group by c.`Name`,c.Age,c.Sex,c.RoomNo,c.BedNo,a.AccidentType ) tempa; ");

            strSql.AppendLine(" select c.`Name`,c.Age,c.Sex,c.RoomNo,c.BedNo,a.AccidentType,count(1) as OccurCount from accidentinfo a ");
            strSql.AppendLine(" inner join guestinfo c on a.GuestID = c.ID ");
            strSql.AppendFormat(" where 1=1 {0} ", strWhere);
            strSql.AppendLine(" group by c.`Name`,c.Age,c.Sex,c.RoomNo,c.BedNo,a.AccidentType  ");

            if (filedOrder.Trim() != "")
            {
                strSql.Append(" order by " + filedOrder);
            }
            strSql.AppendFormat(" LIMIT {0},{1} ", (currentPage - 1) * pageSize, pageSize);

            MySqlParameter[] parameters = {
                        new MySqlParameter("@currentpage", MySqlDbType.Int16),
                        new MySqlParameter("@pagesize", MySqlDbType.Int16)
                        };
            parameters[0].Value = currentPage;
            parameters[1].Value = pageSize;

            return DbHelperSQL.Query(strSql.ToString(), parameters);
        }

        public int UpdateAccident(Accident model)
        {
            List<String> sqlList = new List<string>();
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat(" update accidentinfo set CreateOn='{0}',AccidentType='{1}',Place='{2}',`Condition`='{3}',Remark='{4}' ", model.CreateOn, model.AccidentType, model.Place, model.Condition, model.Remark);
            strSql.AppendFormat(" where AccidentID='{0}' ", model.AccidentID);
            sqlList.Add(strSql.ToString());
            strSql = new StringBuilder();
            strSql.AppendFormat(" delete from accidentrelatedperson where AccidentID='{0}'; ", model.AccidentID);
            sqlList.Add(strSql.ToString());
            strSql = new StringBuilder();
            if (model.ListAccidentRelatedPerson != null && model.ListAccidentRelatedPerson.Count > 0)
            {
                foreach (var item in model.ListAccidentRelatedPerson)
                {
                    strSql.AppendLine(" insert into accidentrelatedperson (ID,AccidentID,StaffNo,StaffName) values ");
                    strSql.AppendFormat(" (null,'{2}','{0}','{1}' ) ; ", item.StaffNo, item.StaffName, model.AccidentID);
                }
            }
            sqlList.Add(strSql.ToString());
            return DbHelperSQL.ExecuteSqlTran(sqlList);
        }
        #endregion

        #region 岗位信息
        public DataSet GetPostInfo()
        {
            DataSet ds = new DataSet();
            string strSql = "select * from postinfo";
            ds = DbHelperSQL.Query(strSql);
            return ds;
        }
        #endregion

        #region 员工信息
        public int AddStaff(Staff model)
        {
            StringBuilder strSql = new StringBuilder();
            var leaveDate = model.LeaveDate == null ? "null" : ("'" + Convert.ToDateTime(model.LeaveDate).ToString("yyyy-MM-dd") + "'");
            var hireDate = model.HireDate == null ? "null" : ("'" + Convert.ToDateTime(model.HireDate).ToString("yyyy-MM-dd") + "'");
            var regularDate = model.RegularDate == null ? "null" : ("'" + Convert.ToDateTime(model.RegularDate).ToString("yyyy-MM-dd") + "'");
            var healthCardDate = model.HealthCardDate == null ? "null" : ("'" + Convert.ToDateTime(model.HealthCardDate).ToString("yyyy-MM-dd") + "'");
            var nurseCardDate = model.NurseCardDate == null ? "null" : ("'" + Convert.ToDateTime(model.NurseCardDate).ToString("yyyy-MM-dd") + "'");
            var contractDate = model.ContractDate == null ? "null" : ("'" + Convert.ToDateTime(model.ContractDate).ToString("yyyy-MM-dd") + "'");
            strSql.AppendFormat(" INSERT into staffinfo(ID,StaffNo,IDCardNo,Name,Sex,PostLevel,Status,LeaveDate,MasterStaffNo,HireDate,RegularDate,Rank,Education,SkillLevel,Phone,EmergencyContactName,EmergencyContactPhone,HealthCardDate,NurseCardLevel,NurseCardDate,ContractDate,NoCrimeProof,StaffOtherNo) ");
            strSql.AppendFormat(" VALUES(null,'','{0}','{1}','{2}','{3}','{4}'," + leaveDate + ",'{5}'," + hireDate + "," + regularDate + ",'{6}','{7}','{8}','{9}','{10}','{11}'," + healthCardDate + ",'{12}'," + nurseCardDate + "," + contractDate + ",'{13}','{14}') ; ",
                model.IDCardNo, model.Name, model.Sex, model.PostLevel, model.Status, model.MasterStaffNo, model.Rank, model.Education, model.SkillLevel, model.Phone, model.EmergencyContactName, model.EmergencyContactPhone, model.NurseCardLevel, model.NoCrimeProof, model.StaffOtherNo);
            //根据postlevel获取工号
            string changeCode = model.PostLevel == 1 ? "A" : model.PostLevel == 2 ? "B" : model.PostLevel == 3 ? "C"
                : model.PostLevel == 4 ? "D" : model.PostLevel == 5 ? "E" : model.PostLevel == 6 ? "F"
                : model.PostLevel == 7 ? "G" : model.PostLevel == 8 ? "H" : model.PostLevel == 9 ? "I" : "J";
            strSql.AppendFormat(" UPDATE staffinfo SET StaffNo=concat('" + changeCode + "','-',LPAD(ID,4,'0')) where ID in (select a.ID from (select MAX(ID) as ID from staffinfo) a) ");
            return DbHelperSQL.ExecuteSql(strSql.ToString());
        }

        public int UpdateStaff(Staff model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine(" UPDATE staffinfo ");
            strSql.AppendFormat(" SET Sex='{0}',PostLevel='{1}',Status='{2}',MasterStaffNo='{3}',Rank='{4}',Education='{5}',SkillLevel='{6}',Phone='{7}',EmergencyContactName='{8}',EmergencyContactPhone='{9}',NurseCardLevel='{10}',StaffOtherNo='{11}' "
                , model.Sex, model.PostLevel, model.Status, model.MasterStaffNo, model.Rank, model.Education, model.SkillLevel, model.Phone, model.EmergencyContactName, model.EmergencyContactPhone, model.NurseCardLevel, model.StaffOtherNo);
            if (!string.IsNullOrWhiteSpace(model.NoCrimeProof))
            {
                strSql.AppendFormat(" ,NoCrimeProof='{0}' ", model.NoCrimeProof);
            }
            if (model.LeaveDate != null)
            {
                strSql.AppendFormat(" ,LeaveDate='{0}' ", model.LeaveDate);
            }
            if (model.HireDate != null)
            {
                strSql.AppendFormat(" ,HireDate='{0}' ", model.HireDate);
            }
            if (model.RegularDate != null)
            {
                strSql.AppendFormat(" ,RegularDate='{0}' ", model.RegularDate);
            }
            if (model.HealthCardDate != null)
            {
                strSql.AppendFormat(" ,HealthCardDate='{0}' ", model.HealthCardDate);
            }
            if (model.NurseCardDate != null)
            {
                strSql.AppendFormat(" ,NurseCardDate='{0}' ", model.NurseCardDate);
            }
            if (model.ContractDate != null)
            {
                strSql.AppendFormat(" ,ContractDate='{0}' ", model.ContractDate);
            }
            strSql.AppendFormat(" where ID='{0}' ", model.ID);
            var result = DbHelperSQL.ExecuteSql(strSql.ToString());
            //若更新成功，则添加员工异动日志
            if (result > 0)
            {
                if ((model.OldPost != model.PostLevel.ToString()) || model.OldRank != model.Rank)
                {
                    string transferType = string.Empty;
                    if (model.OldPost != model.PostLevel.ToString())
                    {
                        transferType = "岗位变更";
                        strSql = new StringBuilder();
                        strSql.AppendLine(" insert into StaffTransfer(ID,StaffNo,TransferType,OldPost,NewPost,OldRank,NewRank,ModifyOn) ");
                        strSql.AppendFormat(" values(null,'{0}','{1}','{2}','{3}','{4}','{5}',now()) ", model.StaffNo, transferType, model.OldPost, model.PostLevel, "", "");
                        DbHelperSQL.ExecuteSql(strSql.ToString());
                    }
                    if (model.OldRank != model.Rank)
                    {
                        transferType = "职级变更";
                        strSql = new StringBuilder();
                        strSql.AppendLine(" insert into StaffTransfer(ID,StaffNo,TransferType,OldPost,NewPost,OldRank,NewRank,ModifyOn) ");
                        strSql.AppendFormat(" values(null,'{0}','{1}','{2}','{3}','{4}','{5}',now()) ", model.StaffNo, transferType, "", "", model.OldRank, model.Rank);
                        DbHelperSQL.ExecuteSql(strSql.ToString());
                    }
                }
            }
            return result;
        }

        public DataSet GetStaffInfoList(int currentPage, int pageSize, string strWhere, string filedOrder)
        {
            //select * from table limit (start-1)*limit,limit; 其中start是页码，limit是每页显示的条数。
            StringBuilder strSql = new StringBuilder();
            //strSql.AppendLine("  select COUNT(1) as totalrow from staffinfo;  ");

            strSql.AppendLine("  select COUNT(1) as totalrow from staffinfo a  ");
            strSql.AppendLine(" inner join postinfo b on a.PostLevel=b.PostLevel ");
            strSql.AppendLine(" left join staffinfo c on a.MasterStaffNo=c.StaffNo ");
            strSql.AppendFormat(" where 1=1 {0} ; ", strWhere);


            strSql.AppendLine(" select a.*,b.PostName,c.Name as MasterStaffName from staffinfo a  ");
            strSql.AppendLine(" inner join postinfo b on a.PostLevel=b.PostLevel ");
            strSql.AppendLine(" left join staffinfo c on a.MasterStaffNo=c.StaffNo ");
            strSql.AppendFormat(" where 1=1 {0} ", strWhere);
            if (filedOrder.Trim() != "")
            {
                strSql.Append(" order by " + filedOrder);
            }
            strSql.AppendFormat(" LIMIT {0},{1} ", (currentPage - 1) * pageSize, pageSize);
            MySqlParameter[] parameters = {
                        new MySqlParameter("@currentpage", MySqlDbType.Int16),
                        new MySqlParameter("@pagesize", MySqlDbType.Int16)
                        };
            parameters[0].Value = currentPage;
            parameters[1].Value = pageSize;
            return DbHelperSQL.Query(strSql.ToString(), parameters);
        }

        public Staff GetStaffInfoByID(int ID)
        {
            Staff model = new Staff();
            string strSql = string.Format(" select * from staffinfo where ID ='{0}' ", ID);
            DataSet ds = new DataSet();
            ds = DbHelperSQL.Query(strSql);
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var row = ds.Tables[0].Rows[0];
                model.ID = ID;
                model.StaffNo = row["StaffNo"].ToString();
                model.IDCardNo = row["IDCardNo"].ToString();
                model.StaffOtherNo = row["StaffOtherNo"].ToString();
                model.MasterStaffNo = row["MasterStaffNo"].ToString();
                model.Name = row["Name"].ToString();
                model.Rank = row["Rank"].ToString();
                model.Sex = Convert.ToInt16(row["Sex"]);
                model.PostLevel = Convert.ToInt16(row["PostLevel"]);
                model.Status = Convert.ToInt16(row["Status"]);

                model.Education = row["Education"].ToString();
                model.SkillLevel = row["SkillLevel"].ToString();
                model.Phone = row["Phone"].ToString();
                model.EmergencyContactName = row["EmergencyContactName"].ToString();
                model.EmergencyContactPhone = row["EmergencyContactPhone"].ToString();
                model.NurseCardLevel = row["NurseCardLevel"].ToString();
                model.NoCrimeProof = row["NoCrimeProof"].ToString();

                if (!string.IsNullOrWhiteSpace(row["LeaveDate"].ToString()))
                {
                    model.LeaveDate = Convert.ToDateTime(row["LeaveDate"]);
                }
                if (!string.IsNullOrWhiteSpace(row["HireDate"].ToString()))
                {
                    model.HireDate = Convert.ToDateTime(row["HireDate"]);
                }
                if (!string.IsNullOrWhiteSpace(row["RegularDate"].ToString()))
                {
                    model.RegularDate = Convert.ToDateTime(row["RegularDate"]);
                }
                if (!string.IsNullOrWhiteSpace(row["HealthCardDate"].ToString()))
                {
                    model.HealthCardDate = Convert.ToDateTime(row["HealthCardDate"]);
                }
                if (!string.IsNullOrWhiteSpace(row["NurseCardDate"].ToString()))
                {
                    model.NurseCardDate = Convert.ToDateTime(row["NurseCardDate"]);
                }
                if (!string.IsNullOrWhiteSpace(row["ContractDate"].ToString()))
                {
                    model.ContractDate = Convert.ToDateTime(row["ContractDate"]);
                }
            }
            return model;
        }

        public string GetStaffNameByStaffNo(string staffNo)
        {
            string name = string.Empty;
            string strSql = string.Format(" select Name from staffinfo where StaffNo ='{0}' ", staffNo);
            DataSet ds = new DataSet();
            ds = DbHelperSQL.Query(strSql);
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                name = ds.Tables[0].Rows[0]["Name"].ToString();
            }
            return name;
        }

        public List<Staff> GetStaffInfoByName(string name)
        {
            List<Staff> listStaff = new List<Staff>();
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine(" select a.*,b.PostName from staffinfo a  ");
            strSql.AppendLine(" inner join postinfo b on a.PostLevel=b.PostLevel ");
            strSql.AppendFormat(" where Name like '%{0}%' ", name);
            DataSet ds = new DataSet();
            ds = DbHelperSQL.Query(strSql.ToString());
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    var ID = ds.Tables[0].Rows[i]["ID"];
                    var staffName = ds.Tables[0].Rows[i]["Name"];
                    var staffNo = ds.Tables[0].Rows[i]["StaffNo"];
                    var staffOtherNo = ds.Tables[0].Rows[i]["StaffOtherNo"];
                    var IDCardNo = ds.Tables[0].Rows[i]["IDCardNo"];
                    var sex = ds.Tables[0].Rows[i]["Sex"];
                    var postName = ds.Tables[0].Rows[i]["PostName"];
                    var rank = ds.Tables[0].Rows[i]["Rank"];
                    listStaff.Add(new Staff()
                    {
                        ID = Convert.ToInt16(ID),
                        Name = staffName.ToString(),
                        Sex = Convert.ToInt16(sex),
                        PostName = postName.ToString(),
                        StaffNo = staffNo.ToString(),
                        IDCardNo = IDCardNo.ToString(),
                        Rank = rank.ToString(),
                        StaffOtherNo = staffOtherNo.ToString()
                    });
                }
            }
            return listStaff;
        }

        public DataSet GetStaffInfoByLevel(string level)
        {
            DataSet ds = new DataSet();
            string strSql = string.Empty;
            if (string.IsNullOrWhiteSpace(level))
            {
                strSql = "select StaffNo,Name,MasterStaffNo,Status from staffinfo where Status <>'2'";
            }
            else
            {
                strSql = "select StaffNo,Name,MasterStaffNo,Status from staffinfo where  Status <>'2' and PostLevel='" + level + "'";
            }
            ds = DbHelperSQL.Query(strSql);
            return ds;
        }

        public DataSet GetStaffInfo()
        {
            DataSet ds = new DataSet();
            string strSql = "select StaffNo,Name from staffinfo ";
            ds = DbHelperSQL.Query(strSql);
            return ds;
        }

        public DataSet GetStaffInfoForExport(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine(" select a.*,b.PostName,c.Name as MasterStaffName from staffinfo a  ");
            strSql.AppendLine(" inner join postinfo b on a.PostLevel=b.PostLevel ");
            strSql.AppendLine(" left join staffinfo c on a.MasterStaffNo=c.StaffNo ");
            strSql.AppendFormat(" where 1=1 {0} ", strWhere);
            return DbHelperSQL.Query(strSql.ToString());
        }

        #endregion

        #region 客人信息
        public DataSet GetGuestInfoList(int currentPage, int pageSize, string strWhere, string filedOrder)
        {
            //select * from table limit (start-1)*limit,limit; 其中start是页码，limit是每页显示的条数。
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat("  select COUNT(1) as totalrow from guestinfo where 1=1 {0} ;  ", strWhere);
            strSql.AppendLine(" select a.* from guestinfo a  ");
            strSql.AppendFormat(" where 1=1 {0} ", strWhere);
            if (filedOrder.Trim() != "")
            {
                strSql.Append(" order by " + filedOrder);
            }
            strSql.AppendFormat(" LIMIT {0},{1} ", (currentPage - 1) * pageSize, pageSize);
            MySqlParameter[] parameters = {
                        new MySqlParameter("@currentpage", MySqlDbType.Int16),
                        new MySqlParameter("@pagesize", MySqlDbType.Int16)
                        };
            parameters[0].Value = currentPage;
            parameters[1].Value = pageSize;
            return DbHelperSQL.Query(strSql.ToString(), parameters);
        }

        public int AddGuest(Guest model)
        {
            StringBuilder strSql = new StringBuilder();
            var leaveDate = model.LeaveDate == null ? "null" : ("'" + Convert.ToDateTime(model.LeaveDate).ToString("yyyy-MM-dd") + "'");
            var changeLevelDate = model.ChangeLevelDate == null ? "null" : ("'" + Convert.ToDateTime(model.ChangeLevelDate).ToString("yyyy-MM-dd") + "'");
            var tryAdmissionDate = model.TryAdmissionDate == null ? "null" : ("'" + Convert.ToDateTime(model.TryAdmissionDate).ToString("yyyy-MM-dd") + "'");
            var birthDay = model.BirthDay == null ? "null" : ("'" + Convert.ToDateTime(model.BirthDay).ToString("yyyy-MM-dd") + "'");

            strSql.AppendLine(" insert into guestinfo(ID,Name,Sex,Age,NurseLevel,RoomNo,BedNo,IDCardNo,AdmissionDate,LeaveDate,Status,ChangeLevelDate,Remark,FloorID,TryAdmissionDate,BirthDay) values ");
            strSql.AppendFormat(" (null,'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}'," + leaveDate + ",'{8}'," + changeLevelDate + ",'{9}','{10}'," + tryAdmissionDate + "," + birthDay + " ) ",
                model.Name, model.Sex, model.Age, model.NurseLevel, model.RoomNo, model.BedNo, model.IDCardNo,
                model.AdmissionDate, model.Status, model.Remark, model.FloorID);
            return DbHelperSQL.ExecuteSql(strSql.ToString());
        }

        public int UpdateGuest(Guest model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine(" update guestinfo ");
            strSql.AppendFormat("  set Sex='{0}',FloorID='{1}',NurseLevel='{2}',RoomNo='{3}',BedNo='{4}',AdmissionDate='{5}',Remark='{6}',Status='{7}' ",
                model.Sex, model.FloorID, model.NurseLevel, model.RoomNo, model.BedNo, model.AdmissionDate, model.Remark, model.Status);
            if (model.ChangeLevelDate != null)
            {
                strSql.AppendFormat("  ,ChangeLevelDate='{0}' ", model.ChangeLevelDate);
            }
            if (model.BirthDay != null)
            {
                strSql.AppendFormat("  ,BirthDay='{0}' ", model.BirthDay);
                strSql.AppendFormat("  ,Age='{0}' ", model.Age);
            }
            if (model.TryAdmissionDate != null)
            {
                strSql.AppendFormat("  ,TryAdmissionDate='{0}' ", model.TryAdmissionDate);
            }
            if (model.LeaveDate != null)
            {
                strSql.AppendFormat("  ,LeaveDate='{0}' ", model.LeaveDate);
            }
            strSql.AppendFormat(" where ID='{0}' ", model.ID);
            return DbHelperSQL.ExecuteSql(strSql.ToString());
        }
        public Guest GetGuestInfoByID(int ID)
        {
            Guest model = new Guest();
            string strSql = string.Format(" select * from guestinfo where ID ='{0}' ", ID);
            DataSet ds = new DataSet();
            ds = DbHelperSQL.Query(strSql);
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var row = ds.Tables[0].Rows[0];
                model.ID = ID;
                model.Name = row["Name"].ToString();
                model.Sex = Convert.ToInt16(row["Sex"]);
                model.Age = Convert.ToInt16(row["Age"]);
                model.NurseLevel = row["NurseLevel"].ToString();
                model.FloorID = row["FloorID"].ToString();
                model.RoomNo = row["RoomNo"].ToString();
                model.BedNo = row["BedNo"].ToString();
                model.IDCardNo = row["IDCardNo"].ToString();
                model.AdmissionDate = Convert.ToDateTime(row["AdmissionDate"]);
                if (!string.IsNullOrWhiteSpace(row["LeaveDate"].ToString()))
                {
                    model.LeaveDate = Convert.ToDateTime(row["LeaveDate"]);
                }
                if (!string.IsNullOrWhiteSpace(row["TryAdmissionDate"].ToString()))
                {
                    model.TryAdmissionDate = Convert.ToDateTime(row["TryAdmissionDate"]);
                }
                if (!string.IsNullOrWhiteSpace(row["BirthDay"].ToString()))
                {
                    model.BirthDay = Convert.ToDateTime(row["BirthDay"]);
                }
                if (!string.IsNullOrWhiteSpace(row["ChangeLevelDate"].ToString()))
                {
                    model.ChangeLevelDate = Convert.ToDateTime(row["ChangeLevelDate"]);
                }
                model.Status = Convert.ToInt16(row["Status"]);
                model.Remark = row["Remark"].ToString();
            }
            return model;
        }

        public List<Guest> GetGuestInfoByName(string name)
        {
            List<Guest> listGuest = new List<Guest>();
            string strSql = string.Format(" select * from guestinfo where Name like '%{0}%' ", name);
            DataSet ds = new DataSet();
            ds = DbHelperSQL.Query(strSql);
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    var guestID = ds.Tables[0].Rows[i]["ID"];
                    var guestName = ds.Tables[0].Rows[i]["Name"];
                    var sex = ds.Tables[0].Rows[i]["Sex"];
                    var age = ds.Tables[0].Rows[i]["Age"];
                    var nurseLevel = ds.Tables[0].Rows[i]["NurseLevel"];
                    var roomNo = ds.Tables[0].Rows[i]["RoomNo"];
                    var bedNo = ds.Tables[0].Rows[i]["BedNo"];
                    var admissionDate = ds.Tables[0].Rows[i]["AdmissionDate"];
                    listGuest.Add(new Guest()
                    {
                        ID = Convert.ToInt16(guestID),
                        Name = guestName.ToString(),
                        Sex = Convert.ToInt16(sex),
                        Age = Convert.ToInt16(age),
                        NurseLevel = nurseLevel.ToString(),
                        RoomNo = roomNo.ToString(),
                        BedNo = bedNo.ToString(),
                        AdmissionDate = Convert.ToDateTime(admissionDate)
                    });
                }
            }
            return listGuest;
        }

        public DataSet GetGuestInfo()
        {
            DataSet ds = new DataSet();
            string strSql = "select ID,Name from guestinfo ";
            ds = DbHelperSQL.Query(strSql);
            return ds;
        }

        public DataSet GetGuestInfoForExport(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine(" select a.* from guestinfo a  ");
            strSql.AppendFormat(" where 1=1 {0} ", strWhere);
            return DbHelperSQL.Query(strSql.ToString());
        }
        #endregion

        #region 日常记录
        public DataSet GetDailyRecordInfoList(int currentPage, int pageSize, string strWhere, string filedOrder)
        {
            //select * from table limit (start-1)*limit,limit; 其中start是页码，limit是每页显示的条数。
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine("  select COUNT(1) as totalrow from(  ");
            strSql.AppendLine(" select a.DailyRecordID,b.RoomNo,b.BedNo,b.Name,a.CreateOn,a.DailyRecordType,a.Remark,a.ReportPerson,d.Name as StaffName from dailyrecordinfo a  ");
            strSql.AppendLine(" left join guestinfo b on a.GuestID = b.ID ");
            strSql.AppendLine(" left join staffinfo d on a.ReportPerson = d.StaffNo ");
            strSql.AppendFormat(" where 1=1 {0} ) tempfortotal;", strWhere);

            strSql.AppendLine(" select a.DailyRecordID,b.RoomNo,b.BedNo,b.Name,a.CreateOn,a.DailyRecordType,a.Remark,a.ReportPerson,d.Name as StaffName from dailyrecordinfo a  ");
            strSql.AppendLine(" left join guestinfo b on a.GuestID = b.ID ");
            strSql.AppendLine(" left join staffinfo d on a.ReportPerson = d.StaffNo ");
            strSql.AppendFormat(" where 1=1 {0} ", strWhere);
            if (filedOrder.Trim() != "")
            {
                strSql.Append(" order by " + filedOrder);
            }
            strSql.AppendFormat(" LIMIT {0},{1} ", (currentPage - 1) * pageSize, pageSize);
            MySqlParameter[] parameters = {
                        new MySqlParameter("@currentpage", MySqlDbType.Int16),
                        new MySqlParameter("@pagesize", MySqlDbType.Int16)
                        };
            parameters[0].Value = currentPage;
            parameters[1].Value = pageSize;
            return DbHelperSQL.Query(strSql.ToString(), parameters);
        }

        public int AddDailyRecord(DailyRecord model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine(" insert into dailyrecordinfo (DailyRecordID,DailyRecordType,GuestID,Remark,CreateOn,ReportPerson) values ");
            strSql.AppendFormat(" (null,'{0}','{1}','{2}','{3}','{4}') ", model.DailyRecordType, model.GuestID, model.Remark, model.CreateOn, model.ReportPerson);
            return DbHelperSQL.ExecuteSql(strSql.ToString());
        }

        public DataSet GetDailyRecordStats(int currentPage, int pageSize, string strWhere, string filedOrder)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine(" select count(1) as totalrow from ( ");
            strSql.AppendLine(" select c.`Name`,c.Sex,c.Age,c.RoomNo,c.BedNo,a.DailyRecordType,count(1) as OccurCount from dailyrecordinfo a ");
            strSql.AppendLine(" inner join guestinfo c on a.GuestID = c.ID ");
            strSql.AppendFormat(" where 1=1 {0} ", strWhere);
            strSql.AppendLine(" group by c.`Name`,c.Sex,c.Age,c.RoomNo,c.BedNo,a.DailyRecordType ) tempa; ");

            strSql.AppendLine(" select c.`Name`,c.Sex,c.Age,c.RoomNo,c.BedNo,a.DailyRecordType,count(1) as OccurCount from dailyrecordinfo a ");
            strSql.AppendLine(" inner join guestinfo c on a.GuestID = c.ID ");
            strSql.AppendFormat(" where 1=1 {0} ", strWhere);
            strSql.AppendLine(" group by c.`Name`,c.Sex,c.Age,c.RoomNo,c.BedNo,a.DailyRecordType  ");

            if (filedOrder.Trim() != "")
            {
                strSql.Append(" order by " + filedOrder);
            }
            strSql.AppendFormat(" LIMIT {0},{1} ", (currentPage - 1) * pageSize, pageSize);

            MySqlParameter[] parameters = {
                        new MySqlParameter("@currentpage", MySqlDbType.Int16),
                        new MySqlParameter("@pagesize", MySqlDbType.Int16)
                        };
            parameters[0].Value = currentPage;
            parameters[1].Value = pageSize;

            return DbHelperSQL.Query(strSql.ToString(), parameters);
        }
        #endregion

        #region 员工考评
        public DataSet GetStaffEvaluateInfoList(int currentPage, int pageSize, string strWhere, string filedOrder)
        {
            //select * from table limit (start-1)*limit,limit; 其中start是页码，limit是每页显示的条数。
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine("  select COUNT(1) as totalrow from staffevaluate;  ");
            strSql.AppendLine(" select a.*,b.Name,b.StaffOtherNo from staffevaluate a  ");
            strSql.AppendLine(" inner join staffinfo b on a.StaffNo = b.StaffNo ");
            strSql.AppendFormat(" where 1=1 {0} ", strWhere);
            if (filedOrder.Trim() != "")
            {
                strSql.Append(" order by " + filedOrder);
            }
            strSql.AppendFormat(" LIMIT {0},{1} ", (currentPage - 1) * pageSize, pageSize);
            MySqlParameter[] parameters = {
                        new MySqlParameter("@currentpage", MySqlDbType.Int16),
                        new MySqlParameter("@pagesize", MySqlDbType.Int16)
                        };
            parameters[0].Value = currentPage;
            parameters[1].Value = pageSize;
            return DbHelperSQL.Query(strSql.ToString(), parameters);
        }

        public int AddStaffEvaluate(StaffEvaluate model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine(" insert into staffevaluate (StaffEvaluateID,StaffNo,EvaluateType,CreateOn,Remark) values ");
            strSql.AppendFormat(" (null,'{0}','{1}','{2}','{3}') ", model.StaffNo, model.EvaluateType, model.CreateOn, model.Remark);
            return DbHelperSQL.ExecuteSql(strSql.ToString());
        }
        #endregion

        #region 房间
        public DataSet GetAllRoom()
        {
            DataSet ds = new DataSet();
            string strSql = "select a.*,(select COUNT(1) from guestinfo where RoomNo = a.RoomNo ) as PeopleNum from room a";
            ds = DbHelperSQL.Query(strSql);
            return ds;
        }
        #endregion

        #region 排班
        public int AddSchedualInfo(List<SchedualInfo> listModel)
        {
            List<string> listSql = new List<string>();
            //删除已经插入的数据（根据员工和日期）
            var maxDate = listModel.Max(p => Convert.ToDateTime(p.DutyDate));
            var minDate = listModel.Min(p => Convert.ToDateTime(p.DutyDate));
            StringBuilder delWhere = new StringBuilder();
            List<string> listStaff = new List<string>();
            var staffs = listModel.GroupBy(p => p.StaffNo);
            foreach (var item in staffs)
            {
                listStaff.Add(item.Key);
            }
            if (listStaff.Count > 0)
            {
                delWhere.AppendFormat(" and DutyDate >='{0}' and DutyDate <='{1}' ", minDate, maxDate);
                for (int i = 0; i < listStaff.Count; i++)
                {
                    if (i == 0)
                    {
                        delWhere.AppendFormat(" and StaffNo in ('{0}' ", listStaff[i]);
                    }
                    else
                    {
                        delWhere.AppendFormat(" ,'{0}' ", listStaff[i]);
                    }
                }
                delWhere.Append(") ");
            }

            listSql.Add("delete from schedualInfo where 1=1 " + delWhere.ToString() + " ");
            foreach (var item in listModel)
            {
                var beginTime = item.BeginTime == null ? "null" : ("'" + Convert.ToDateTime(item.BeginTime).ToString("yyyy-MM-dd HH:mm:ss") + "'");
                var endTime = item.EndTime == null ? "null" : ("'" + Convert.ToDateTime(item.EndTime).ToString("yyyy-MM-dd HH:mm:ss") + "'");
                string sql = string.Format("insert into schedualInfo(StaffNo,DutyDate,Schedual,RoomNo,Hours,Remark,BeginTime,EndTime,IsNight) values ('{0}','{1}','{2}','{3}','{4}','{5}'," + beginTime + "," + endTime + ",'{6}')", item.StaffNo, item.DutyDate, item.Schedual, item.RoomNo, item.Hours, item.Remark, item.IsNight);
                listSql.Add(sql);
            }
            return DbHelperSQL.ExecuteSqlTran(listSql);
        }
        public DataSet GetSchedualInfo(string sqlWhere)
        {
            DataSet ds = new DataSet();
            string strSql = string.Format(" select a.*,b.`Name` from schedualinfo a inner join staffinfo b on a.StaffNo = b.StaffNo where 1=1 {0} order by a.RoomNo", sqlWhere);
            ds = DbHelperSQL.Query(strSql);
            return ds;
        }
        public bool VerifySchedualInfo(List<SchedualInfo> listModel)
        {
            var maxDate = listModel.Max(p => Convert.ToDateTime(p.DutyDate));
            var minDate = listModel.Min(p => Convert.ToDateTime(p.DutyDate));
            List<string> rooms = new List<string>();
            foreach (var item in listModel)
            {
                if (!rooms.Contains(item.RoomNo) && item.RoomNo != "")
                {
                    rooms.Add(item.RoomNo);
                }
            }
            if (rooms.Count > 0)
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendFormat(" select COUNT(1) as totalrow from schedualinfo where DutyDate>='{0}' and DutyDate<='{1}' ", minDate, maxDate);
                for (int i = 0; i < rooms.Count; i++)
                {
                    if (i == 0)
                    {
                        strSql.AppendFormat(" and RoomNo in ('{0}' ", rooms[i]);
                    }
                    else
                    {
                        strSql.AppendFormat(" ,'{0}' ", rooms[i]);
                    }
                }
                strSql.Append(") ");
                DataSet ds = new DataSet();
                ds = DbHelperSQL.Query(strSql.ToString());
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var rowNum = ds.Tables[0].Rows[0]["totalrow"].ToString();
                    if (Convert.ToInt16(rowNum) > 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        #endregion

        #region 排班模板
        public DataSet GetSchedualTemplateList(int currentPage, int pageSize, string strWhere, string filedOrder)
        {
            //select * from table limit (start-1)*limit,limit; 其中start是页码，limit是每页显示的条数。
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine("  select COUNT(1) as totalrow from schedualtemplate;  ");
            strSql.AppendLine(" select a.* from schedualtemplate a  ");
            strSql.AppendFormat(" where 1=1 {0} ", strWhere);
            if (filedOrder.Trim() != "")
            {
                strSql.Append(" order by " + filedOrder);
            }
            strSql.AppendFormat(" LIMIT {0},{1} ", (currentPage - 1) * pageSize, pageSize);
            MySqlParameter[] parameters = {
                        new MySqlParameter("@currentpage", MySqlDbType.Int16),
                        new MySqlParameter("@pagesize", MySqlDbType.Int16)
                        };
            parameters[0].Value = currentPage;
            parameters[1].Value = pageSize;
            return DbHelperSQL.Query(strSql.ToString(), parameters);
        }
        public int AddSchedualTemplate(SchedualTemplate model)
        {
            List<String> sqlList = new List<string>();
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine(" insert into schedualtemplate (ID,TemplateName,RoomList,PeopleNum,TemplateRemark,Status) values ");
            strSql.AppendFormat(" (null,'{0}','{1}','{2}','{3}','{4}') ", model.TemplateName, model.RoomList, model.PeopleNum, model.TemplateRemark, model.Status);
            sqlList.Add(strSql.ToString());
            strSql = new StringBuilder();
            if (model.ListTemplateMapping != null && model.ListTemplateMapping.Count > 0)
            {
                foreach (var item in model.ListTemplateMapping)
                {
                    strSql.AppendLine(" insert into templatemapping (TemplateID,SerialNo,WhichDay,Schedual,Remark) values ");
                    strSql.AppendFormat(" ((select a.ID from (select MAX(ID) as ID from schedualtemplate) a),'{0}','{1}','{2}','{3}' ) ; ", item.SerialNo, item.WhichDay, item.Schedual, item.Remark);
                }
            }
            sqlList.Add(strSql.ToString());
            return DbHelperSQL.ExecuteSqlTran(sqlList);
        }
        public int UpdateSchedualTemplate(SchedualTemplate model)
        {
            List<String> sqlList = new List<string>();
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat(" update schedualtemplate set TemplateName='{1}',RoomList='{2}',PeopleNum='{3}',TemplateRemark='{4}',Status='{5}' where ID='{0}' ", model.ID, model.TemplateName, model.RoomList, model.PeopleNum, model.TemplateRemark, model.Status);
            sqlList.Add(strSql.ToString());
            strSql = new StringBuilder();
            if (model.ListTemplateMapping != null && model.ListTemplateMapping.Count > 0)
            {
                strSql.AppendLine(" delete from templatemapping where TemplateID='" + model.ID + "';  ");
                sqlList.Add(strSql.ToString());
                strSql = new StringBuilder();
                foreach (var item in model.ListTemplateMapping)
                {
                    strSql.AppendLine(" insert into templatemapping (TemplateID,SerialNo,WhichDay,Schedual,Remark) values ");
                    strSql.AppendFormat(" ('{0}','{1}','{2}','{3}','{4}' ) ; ", model.ID, item.SerialNo, item.WhichDay, item.Schedual, item.Remark);
                }
            }
            sqlList.Add(strSql.ToString());
            return DbHelperSQL.ExecuteSqlTran(sqlList);
        }
        public bool VerifySchedualTemplate(SchedualTemplate model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat(" select COUNT(1) as totalrow from schedualtemplate where TemplateName='{0}' ", model.TemplateName);
            DataSet ds = new DataSet();
            ds = DbHelperSQL.Query(strSql.ToString());
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var rowNum = ds.Tables[0].Rows[0]["totalrow"].ToString();
                if (Convert.ToInt16(rowNum) > 0)
                {
                    return false;
                }
            }
            return true;
        }

        public SchedualTemplate GetSchedualTemplateByID(string ID)
        {
            SchedualTemplate model = new SchedualTemplate();
            string strSql = string.Format(" select * from schedualtemplate a left join templatemapping b on a.ID = b.TemplateID where a.ID ='{0}' ", ID);
            DataSet ds = new DataSet();
            ds = DbHelperSQL.Query(strSql);
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var row_0 = ds.Tables[0].Rows[0];
                model.ID = Convert.ToInt16(ID);
                model.TemplateName = row_0["TemplateName"].ToString();
                model.RoomList = row_0["RoomList"].ToString();
                model.PeopleNum = Convert.ToInt16(row_0["PeopleNum"]);
                model.TemplateRemark = row_0["TemplateRemark"].ToString();
                model.Status = Convert.ToInt16(row_0["Status"]);
                List<TemplateMapping> listModel = new List<TemplateMapping>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    var row = ds.Tables[0].Rows[i];
                    listModel.Add(new TemplateMapping()
                    {
                        TemplateID = Convert.ToInt16(ID),
                        SerialNo = Convert.ToInt16(row["SerialNo"]),
                        WhichDay = Convert.ToInt16(row["WhichDay"]),
                        Schedual = row["Schedual"].ToString(),
                        Remark = row["Remark"].ToString()
                    });
                }
                model.ListTemplateMapping = listModel;
            }
            return model;
        }

        public DataSet GetSchedualTemplateInfo()
        {
            DataSet ds = new DataSet();
            string strSql = "select * from schedualtemplate where `Status`=1 ";
            ds = DbHelperSQL.Query(strSql);
            return ds;
        }
        #endregion

        #region 其他工时信息
        public DataSet GetOtherWorkTimeList(int currentPage, int pageSize, string strWhere, string filedOrder)
        {
            //select * from table limit (start-1)*limit,limit; 其中start是页码，limit是每页显示的条数。
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine("  select COUNT(1) as totalrow from otherWorkTime;  ");
            strSql.AppendLine(" select a.*,b.Name from otherworktime a inner join staffinfo b on a.StaffNo = b.StaffNo ");
            strSql.AppendFormat(" where 1=1 {0} ", strWhere);
            if (filedOrder.Trim() != "")
            {
                strSql.Append(" order by " + filedOrder);
            }
            strSql.AppendFormat(" LIMIT {0},{1} ", (currentPage - 1) * pageSize, pageSize);
            MySqlParameter[] parameters = {
                        new MySqlParameter("@currentpage", MySqlDbType.Int16),
                        new MySqlParameter("@pagesize", MySqlDbType.Int16)
                        };
            parameters[0].Value = currentPage;
            parameters[1].Value = pageSize;
            return DbHelperSQL.Query(strSql.ToString(), parameters);
        }

        public int AddOtherWorkTime(OtherWorkTime model)
        {
            StringBuilder strSql = new StringBuilder();
            var beginTime = model.BeginTime == null ? "null" : ("'" + Convert.ToDateTime(model.BeginTime).ToString("yyyy-MM-dd HH:mm") + "'");
            var endTime = model.EndTime == null ? "null" : ("'" + Convert.ToDateTime(model.EndTime).ToString("yyyy-MM-dd HH:mm") + "'");

            strSql.AppendLine(" insert into otherworktime(ID,StaffNo,WorkType,BeginTime,EndTime,Hours,Status,Remark) values ");
            strSql.AppendFormat(" (null,'{0}','{1}'," + beginTime + "," + endTime + ",'{2}','{3}','{4}' ) ",
                model.StaffNo, model.WorkType, model.Hours, model.Status, model.Remark);
            return DbHelperSQL.ExecuteSql(strSql.ToString());
        }

        public int UpdateOtherWorkTime(OtherWorkTime model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine(" update otherworktime ");
            strSql.AppendFormat("  set StaffNo='{0}',WorkType='{1}',Hours='{2}',Status='{3}',Remark='{4}' ",
                model.StaffNo, model.WorkType, model.Hours, model.Status, model.Remark);
            if (model.BeginTime != null)
            {
                strSql.AppendFormat("  ,BeginTime='{0}' ", model.BeginTime);
            }
            if (model.EndTime != null)
            {
                strSql.AppendFormat("  ,EndTime='{0}' ", model.EndTime);
            }
            strSql.AppendFormat(" where ID='{0}' ", model.ID);
            return DbHelperSQL.ExecuteSql(strSql.ToString());
        }
        public OtherWorkTime GetOtherWorkTimeByID(int ID)
        {
            OtherWorkTime model = new OtherWorkTime();
            string strSql = string.Format(" select * from otherworktime where ID ='{0}' ", ID);
            DataSet ds = new DataSet();
            ds = DbHelperSQL.Query(strSql);
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var row = ds.Tables[0].Rows[0];
                model.ID = ID;
                model.StaffNo = row["StaffNo"].ToString();
                model.WorkType = Convert.ToInt16(row["WorkType"]);
                model.Hours = Convert.ToInt16(row["Hours"]);
                model.Status = Convert.ToInt16(row["Status"]);
                model.Remark = row["Remark"].ToString();
                if (!string.IsNullOrWhiteSpace(row["BeginTime"].ToString()))
                {
                    model.BeginTime = Convert.ToDateTime(row["BeginTime"]);
                }
                if (!string.IsNullOrWhiteSpace(row["EndTime"].ToString()))
                {
                    model.EndTime = Convert.ToDateTime(row["EndTime"]);
                }
            }
            return model;
        }

        public DataSet GetSchedualCountList(int currentPage, int pageSize, string strWhere_a, string strWhere_b, string filedOrder)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine(" select COUNT(1) as totalrow from ( ");
            strSql.AppendLine(" select temp.StaffNo,temp.`Name`,SUM(temp.NormalHours) as NormalHours,SUM(temp.OtherHours) OtherHours from ( ");
            strSql.AppendLine(" select a.StaffNo,b.`Name`,SUM(a.Hours) as NormalHours,0 as OtherHours from schedualinfo a  ");
            strSql.AppendLine(" inner join staffinfo b on a.StaffNo = b.StaffNo ");
            strSql.AppendFormat(" where 1=1 {0} ", strWhere_a);
            strSql.AppendLine(" group by a.StaffNo,b.`Name`  ");
            strSql.AppendLine(" union all ");
            strSql.AppendLine(" select a.StaffNo,b.`Name`,0 as NormalHours,SUM(a.Hours) as OtherHours from otherworktime a  ");
            strSql.AppendLine(" inner join staffinfo b on a.StaffNo = b.StaffNo  ");
            strSql.AppendFormat(" where 1=1 {0} ", strWhere_b);
            strSql.AppendLine(" group by a.StaffNo,b.`Name`) temp group by temp.StaffNo,temp.`Name` order by (temp.NormalHours+temp.OtherHours) desc ) temptb;  ");

            strSql.AppendLine(" select temp.StaffNo,temp.`Name`,SUM(temp.NormalHours) as NormalHours,SUM(temp.OtherHours) OtherHours from ( ");
            strSql.AppendLine(" select a.StaffNo,b.`Name`,SUM(a.Hours) as NormalHours,0 as OtherHours from schedualinfo a  ");
            strSql.AppendLine(" inner join staffinfo b on a.StaffNo = b.StaffNo ");
            strSql.AppendFormat(" where 1=1 {0} ", strWhere_a);
            strSql.AppendLine(" group by a.StaffNo,b.`Name`  ");
            strSql.AppendLine(" union all ");
            strSql.AppendLine(" select a.StaffNo,b.`Name`,0 as NormalHours,SUM(a.Hours) as OtherHours from otherworktime a  ");
            strSql.AppendLine(" inner join staffinfo b on a.StaffNo = b.StaffNo  ");
            strSql.AppendFormat(" where 1=1 {0} ", strWhere_b);
            strSql.AppendLine(" group by a.StaffNo,b.`Name`) temp group by temp.StaffNo,temp.`Name` ");
            strSql.AppendLine(" order by (temp.NormalHours+temp.OtherHours) desc ");
            //if (filedOrder.Trim() != "")
            //{
            //    strSql.Append(" order by " + filedOrder);
            //}
            strSql.AppendFormat(" LIMIT {0},{1} ", (currentPage - 1) * pageSize, pageSize);
            MySqlParameter[] parameters = {
                        new MySqlParameter("@currentpage", MySqlDbType.Int16),
                        new MySqlParameter("@pagesize", MySqlDbType.Int16)
                        };
            parameters[0].Value = currentPage;
            parameters[1].Value = pageSize;
            return DbHelperSQL.Query(strSql.ToString(), parameters);
        }
        #endregion

        #region 班次维护
        public DataSet GetSchedualTime()
        {
            DataSet ds = new DataSet();
            string strSql = "select * from SchedualTime";
            ds = DbHelperSQL.Query(strSql);
            return ds;
        }
        public DataSet GetSchedualTimeList(int currentPage, int pageSize, string strWhere, string filedOrder)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine("  select COUNT(1) as totalrow from schedualtime;  ");
            strSql.AppendLine(" select * from schedualtime ");
            if (filedOrder.Trim() != "")
            {
                strSql.Append(" order by " + filedOrder);
            }
            strSql.AppendFormat(" LIMIT {0},{1} ", (currentPage - 1) * pageSize, pageSize);
            MySqlParameter[] parameters = {
                        new MySqlParameter("@currentpage", MySqlDbType.Int16),
                        new MySqlParameter("@pagesize", MySqlDbType.Int16)
                        };
            parameters[0].Value = currentPage;
            parameters[1].Value = pageSize;
            return DbHelperSQL.Query(strSql.ToString(), parameters);
        }

        public int DeleteSchedualTime(int ID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat(" delete from schedualtime where ID ='{0}' ", ID);
            return DbHelperSQL.ExecuteSql(strSql.ToString());
        }

        public int AddSchedualTime(string name, string schedual)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine(" insert into schedualtime values ");
            strSql.AppendFormat(" (null,'{0}','{1}' ) ", name, schedual);
            return DbHelperSQL.ExecuteSql(strSql.ToString());
        }

        public bool ExistsSchedualTime(string name)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat(" select count(1) from schedualtime where ID='{0}'", name);
            return DbHelperSQL.Exists(strSql.ToString());
        }
        #endregion

        #region 房间组合
        public DataSet GetRoomCombine()
        {
            DataSet ds = new DataSet();
            string strSql = "select * from roomcombine";
            ds = DbHelperSQL.Query(strSql);
            return ds;
        }
        public DataSet GetRoomCombineList(int currentPage, int pageSize, string strWhere, string filedOrder)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat("  select COUNT(1) as totalrow from roomcombine where 1=1 {0} ;  ", strWhere);
            strSql.AppendLine(" select * from roomcombine ");
            strSql.AppendFormat(" where 1=1 {0} ", strWhere);
            if (filedOrder.Trim() != "")
            {
                strSql.Append(" order by " + filedOrder);
            }
            strSql.AppendFormat(" LIMIT {0},{1} ", (currentPage - 1) * pageSize, pageSize);
            MySqlParameter[] parameters = {
                        new MySqlParameter("@currentpage", MySqlDbType.Int16),
                        new MySqlParameter("@pagesize", MySqlDbType.Int16)
                        };
            parameters[0].Value = currentPage;
            parameters[1].Value = pageSize;
            return DbHelperSQL.Query(strSql.ToString(), parameters);
        }

        public int DeleteRoomCombine(int ID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat(" delete from roomcombine where ID ='{0}' ", ID);
            return DbHelperSQL.ExecuteSql(strSql.ToString());
        }

        public int AddRoomCombine(RoomCombine model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine(" insert into roomcombine values ");
            strSql.AppendFormat(" (null,'{0}','{1}' ) ", model.RoomList, model.Remark);
            return DbHelperSQL.ExecuteSql(strSql.ToString());
        }

        public bool ExistsRoomCombine(string name)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat(" select count(1) from roomcombine where RoomList='{0}'", name);
            return DbHelperSQL.Exists(strSql.ToString());
        }

        public RoomCombine GetRoomCombineInfoByID(int ID)
        {
            RoomCombine model = new RoomCombine();
            string strSql = string.Format(" select * from roomcombine where ID ='{0}' ", ID);
            DataSet ds = new DataSet();
            ds = DbHelperSQL.Query(strSql);
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var row = ds.Tables[0].Rows[0];
                model.ID = ID;
                model.RoomList = row["RoomList"].ToString();
                model.Remark = row["Remark"].ToString();
            }
            return model;
        }

        public int UpdateRoomCombine(RoomCombine model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine(" update roomcombine ");
            strSql.AppendFormat("  set RoomList='{0}',Remark='{1}' ", model.RoomList, model.Remark);
            strSql.AppendFormat(" where ID='{0}' ", model.ID);
            return DbHelperSQL.ExecuteSql(strSql.ToString());
        }
        #endregion

        #region 考核模板信息
        public DataSet GetAssessTemplateList(int currentPage, int pageSize, string strWhere, string filedOrder)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat("  select COUNT(1) as totalrow from assesstemplate where 1=1 {0} ;  ", strWhere);
            strSql.AppendLine(" select a.*,b.PostName from assesstemplate a left join postinfo b on a.Post=b.PostLevel ");
            strSql.AppendFormat(" where 1=1 {0} ", strWhere);
            if (filedOrder.Trim() != "")
            {
                strSql.Append(" order by " + filedOrder);
            }
            strSql.AppendFormat(" LIMIT {0},{1} ", (currentPage - 1) * pageSize, pageSize);
            MySqlParameter[] parameters = {
                        new MySqlParameter("@currentpage", MySqlDbType.Int16),
                        new MySqlParameter("@pagesize", MySqlDbType.Int16)
                        };
            parameters[0].Value = currentPage;
            parameters[1].Value = pageSize;
            return DbHelperSQL.Query(strSql.ToString(), parameters);
        }

        public int AddAssessTemplate(AssessTemplate model)
        {
            List<String> sqlList = new List<string>();
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine(" insert into assesstemplate(ID,Rank,AssessType,Status,Post) values ");
            strSql.AppendFormat(" (null,'{0}','{1}','{2}','{3}' ); ", model.Rank, model.AssessType, model.Status, model.Post);
            sqlList.Add(strSql.ToString());
            foreach (var item in model.ListAssessTemplateDetail)
            {
                strSql = new StringBuilder();
                strSql.AppendLine(" insert into assesstemplatedetail(DetailID,AssessTemplateID,AssessProjectType,AssessProjectNo,AssessProjectName,Remark,Score) values ");
                strSql.AppendFormat(" (null,(select a.ID from (select MAX(ID) as ID from assesstemplate) a),'{0}','{1}','{2}','{3}','{4}' ); ",
                    item.AssessProjectType, item.AssessProjectNo, item.AssessProjectName, item.Remark, item.Score);
                sqlList.Add(strSql.ToString());
            }
            return DbHelperSQL.ExecuteSqlTran(sqlList);
        }

        public int UpdateAssessTemplate(AssessTemplate model)
        {
            List<String> sqlList = new List<string>();
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat(" update assesstemplate set Rank='{0}',AssessType='{1}',Post='{2}',Status='{3}' ", model.Rank, model.AssessType, model.Post, model.Status);
            strSql.AppendFormat(" where ID='{0}'; ", model.ID);
            sqlList.Add(strSql.ToString());
            strSql = new StringBuilder();
            strSql.AppendFormat(" delete from assesstemplatedetail where AssessTemplateID='{0}'; ", model.ID);
            sqlList.Add(strSql.ToString());
            foreach (var item in model.ListAssessTemplateDetail)
            {
                strSql = new StringBuilder();
                strSql.AppendLine(" insert into assesstemplatedetail(DetailID,AssessTemplateID,AssessProjectType,AssessProjectNo,AssessProjectName,Remark,Score) values ");
                strSql.AppendFormat(" (null,'{0}','{1}','{2}','{3}','{4}','{5}' ); ",
                   model.ID, item.AssessProjectType, item.AssessProjectNo, item.AssessProjectName, item.Remark, item.Score);
                sqlList.Add(strSql.ToString());
            }
            return DbHelperSQL.ExecuteSqlTran(sqlList);
        }
        public AssessTemplate GetAssessTemplateByID(int ID)
        {
            AssessTemplate model = new AssessTemplate();
            string strSql = string.Format(" select * from assesstemplate a left join assesstemplatedetail b on a.ID=b.AssessTemplateID where a.ID ='{0}' order by b.DetailID ", ID);
            DataSet ds = new DataSet();
            ds = DbHelperSQL.Query(strSql);
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var row_0 = ds.Tables[0].Rows[0];
                model.ID = ID;
                model.Rank = row_0["Rank"].ToString();
                model.Post = Convert.ToInt16(row_0["Post"]);
                model.AssessType = row_0["AssessType"].ToString();
                model.Status = Convert.ToInt16(row_0["Status"]);
                List<AssessTemplateDetail> details = new List<AssessTemplateDetail>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    var row = ds.Tables[0].Rows[i];
                    details.Add(new AssessTemplateDetail()
                    {
                        DetailID = Convert.ToInt16(row["DetailID"]),
                        AssessTemplateID = Convert.ToInt16(row["AssessTemplateID"]),
                        AssessProjectType = row["AssessProjectType"].ToString(),
                        AssessProjectNo = row["AssessProjectNo"].ToString(),
                        AssessProjectName = row["AssessProjectName"].ToString(),
                        Score = Convert.ToDecimal(row["Score"]),
                        Remark = row["Remark"].ToString()
                    });
                }
                model.ListAssessTemplateDetail = details;
            }
            return model;
        }
        public int DeleteAssessTemplate(int ID)
        {
            List<String> sqlList = new List<string>();
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat(" delete from assesstemplate where ID ='{0}'; ", ID);
            sqlList.Add(strSql.ToString());
            strSql = new StringBuilder();
            strSql.AppendFormat(" delete from assesstemplatedetail where AssessTemplateID ='{0}'; ", ID);
            sqlList.Add(strSql.ToString());
            return DbHelperSQL.ExecuteSqlTran(sqlList);
        }
        public bool ExistsAssessTemplate(AssessTemplate model)
        {
            StringBuilder strSql = new StringBuilder();
            if (model.ID > 0)
            {
                strSql.AppendFormat(" select count(1) from assesstemplate where Rank='{0}' and AssessType='{1}' and Post='{2}' and ID <>'{3}' ", model.Rank, model.AssessType, model.Post, model.ID);
            }
            else
            {
                strSql.AppendFormat(" select count(1) from assesstemplate where Rank='{0}' and AssessType='{1}' and Post='{2}' ", model.Rank, model.AssessType, model.Post);
            }
            return DbHelperSQL.Exists(strSql.ToString());
        }
        public bool ExistsHasStaffAssess(AssessTemplate model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat(" select count(1) from staffassess where AssessTemplateID='{0}' ", model.ID);
            return DbHelperSQL.Exists(strSql.ToString());
        }
        public DataSet GetAssessTemplate(string strWhere)
        {
            DataSet ds = new DataSet();
            string strSql = "select * from assesstemplate where 1=1 " + strWhere;
            ds = DbHelperSQL.Query(strSql);
            return ds;
        }
        public AssessTemplate GetAssessTemplateByWhere(string strWhere)
        {
            AssessTemplate model = new AssessTemplate();
            string strSql = string.Format(" select * from assesstemplate a left join assesstemplatedetail b on a.ID=b.AssessTemplateID where 1=1 {0}  order by b.DetailID  ", strWhere);
            DataSet ds = new DataSet();
            ds = DbHelperSQL.Query(strSql);
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var row_0 = ds.Tables[0].Rows[0];
                model.ID = Convert.ToInt16(row_0["ID"]);
                model.Rank = row_0["Rank"].ToString();
                model.AssessType = row_0["AssessType"].ToString();
                model.Status = Convert.ToInt16(row_0["Status"]);
                List<AssessTemplateDetail> details = new List<AssessTemplateDetail>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    var row = ds.Tables[0].Rows[i];
                    details.Add(new AssessTemplateDetail()
                    {
                        DetailID = Convert.ToInt16(row["DetailID"]),
                        AssessTemplateID = Convert.ToInt16(row["AssessTemplateID"]),
                        AssessProjectType = row["AssessProjectType"].ToString(),
                        AssessProjectNo = row["AssessProjectNo"].ToString(),
                        AssessProjectName = row["AssessProjectName"].ToString(),
                        Score = Convert.ToDecimal(row["Score"]),
                        Remark = row["Remark"].ToString()
                    });
                }
                model.ListAssessTemplateDetail = details;
            }
            return model;
        }
        #endregion

        #region 员工绩效考核
        public DataSet GetStaffAssessList(int currentPage, int pageSize, string strWhere, string filedOrder)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine(" select COUNT(1) as totalrow from ");
            strSql.AppendLine(" (select a.*,b.AssessType,c.`Name`,c.Rank from staffassess a ");
            strSql.AppendLine(" inner join assesstemplate b on a.AssessTemplateID = b.ID ");
            strSql.AppendLine(" inner join staffinfo c on a.StaffNo = c.StaffNo ");
            strSql.AppendFormat(" where 1=1 {0} ", strWhere);
            strSql.AppendLine(" )tempCount; ");
            strSql.AppendLine(" select a.*,b.AssessType,c.`Name`,c.Rank from staffassess a ");
            strSql.AppendLine(" inner join assesstemplate b on a.AssessTemplateID = b.ID ");
            strSql.AppendLine(" inner join staffinfo c on a.StaffNo = c.StaffNo ");
            strSql.AppendFormat(" where 1=1 {0} ", strWhere);
            if (filedOrder.Trim() != "")
            {
                strSql.Append(" order by " + filedOrder);
            }
            strSql.AppendFormat(" LIMIT {0},{1} ", (currentPage - 1) * pageSize, pageSize);
            MySqlParameter[] parameters = {
                        new MySqlParameter("@currentpage", MySqlDbType.Int16),
                        new MySqlParameter("@pagesize", MySqlDbType.Int16)
                        };
            parameters[0].Value = currentPage;
            parameters[1].Value = pageSize;
            return DbHelperSQL.Query(strSql.ToString(), parameters);
        }

        public int AddStaffAssess(StaffAssess model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine(" insert into staffassess(ID,StaffNo,AssessTemplateID,Score,AssessDate,ModifyOn,Status,Remark) values ");
            strSql.AppendFormat(" (null,'{0}','{1}','{2}','{3}',now(),'{4}','{5}' ) ",
                model.StaffNo, model.AssessTemplateID, model.Score, model.AssessDate.ToString("yyyy-MM-dd"), model.Status, model.Remark);
            return DbHelperSQL.ExecuteSql(strSql.ToString());
        }

        public int UpdateStaffAssess(StaffAssess model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine(" update staffassess ");
            strSql.AppendFormat("  set Score='{0}',AssessDate='{1}',ModifyOn=now(),Status='{2}',Remark='{3}' ",
                model.Score, model.AssessDate.ToString("yyyy-MM-dd"), model.Status, model.Remark);
            strSql.AppendFormat(" where ID='{0}' ", model.ID);
            return DbHelperSQL.ExecuteSql(strSql.ToString());
        }
        public StaffAssess GetStaffAssessByID(int ID)
        {
            StaffAssess model = new StaffAssess();
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine(" select a.*,b.AssessType,c.`Name`,c.Rank,d.AssessProjectType,d.AssessProjectNo,d.AssessProjectName,d.Remark as TempLateRemark,d.Score as FullScore from staffassess a ");
            strSql.AppendLine(" inner join assesstemplate b on a.AssessTemplateID = b.ID ");
            strSql.AppendLine(" inner join staffinfo c on a.StaffNo = c.StaffNo ");
            strSql.AppendLine(" inner join assesstemplatedetail d on a.AssessTemplateID = d.AssessTemplateID ");
            strSql.AppendFormat(" where a.ID='{0}' order by d.DetailID ", ID);
            DataSet ds = new DataSet();
            ds = DbHelperSQL.Query(strSql.ToString());
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var row_0 = ds.Tables[0].Rows[0];
                model.ID = ID;
                model.StaffNo = row_0["StaffNo"].ToString();
                model.AssessTemplateID = Convert.ToInt16(row_0["AssessTemplateID"]);
                model.AssessDate = Convert.ToDateTime(row_0["AssessDate"]);
                model.AssessType = row_0["AssessType"].ToString();
                model.Status = Convert.ToInt16(row_0["Status"]);
                model.Remark = row_0["Remark"].ToString();
                model.Name = row_0["Name"].ToString();
                model.Rank = row_0["Rank"].ToString();
                model.ModifyOn = Convert.ToDateTime(row_0["ModifyOn"]);
                model.Score = row_0["Score"].ToString();
                List<AssessTemplateDetail> details = new List<AssessTemplateDetail>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    var row = ds.Tables[0].Rows[i];
                    details.Add(new AssessTemplateDetail()
                    {
                        AssessProjectType = row["AssessProjectType"].ToString(),
                        AssessProjectNo = row["AssessProjectNo"].ToString(),
                        AssessProjectName = row["AssessProjectName"].ToString(),
                        Score = Convert.ToDecimal(row["FullScore"]),
                        Remark = row["TempLateRemark"].ToString()
                    });
                }
                model.ListAssessTemplateDetail = details;
            }
            return model;
        }
        public int DeleteStaffAssess(int ID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat(" delete from staffassess where ID ='{0}' ", ID);
            return DbHelperSQL.ExecuteSql(strSql.ToString());
        }
        public bool ExistsStaffAssess(StaffAssess model)
        {
            StringBuilder strSql = new StringBuilder();
            if (model.ID > 0)
            {
                strSql.AppendFormat(" select count(1) from staffassess where StaffNo='{0}' and AssessTemplateID='{1}' and ID <>'{2}' ", model.StaffNo, model.AssessTemplateID, model.ID);
            }
            else
            {
                strSql.AppendFormat(" select count(1) from staffassess where StaffNo='{0}' and AssessTemplateID='{1}' ", model.StaffNo, model.AssessTemplateID);
            }
            return DbHelperSQL.Exists(strSql.ToString());
        }

        public DataSet GetStaffAssess(string strWhere)
        {
            DataSet ds = new DataSet();
            string strSql = "select * from staffassess where 1=1 " + strWhere;
            ds = DbHelperSQL.Query(strSql);
            return ds;
        }

        public DataSet GetStaffAssessForExport(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine(" select a.*,b.AssessType,c.`Name`,c.Rank from staffassess a ");
            strSql.AppendLine(" inner join assesstemplate b on a.AssessTemplateID = b.ID ");
            strSql.AppendLine(" inner join staffinfo c on a.StaffNo = c.StaffNo ");
            strSql.AppendFormat(" where 1=1 {0} ", strWhere);
            return DbHelperSQL.Query(strSql.ToString());
        }
        #endregion

        #region 参数码表
        public DataSet GetParamOption(string type)
        {
            string strSql = string.Format(" select * from ParamOption where ParamType ='{0}' ", type);
            DataSet ds = new DataSet();
            ds = DbHelperSQL.Query(strSql);
            return ds;
        }

        public DataSet GetBasicInfoList(int currentPage, int pageSize, string strWhere, string filedOrder)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat("  select COUNT(1) as totalrow from paramoption where 1=1 {0} ;  ", strWhere);
            strSql.AppendFormat(" select * from paramoption where 1=1 {0} ", strWhere);
            if (filedOrder.Trim() != "")
            {
                strSql.Append(" order by " + filedOrder);
            }
            strSql.AppendFormat(" LIMIT {0},{1} ", (currentPage - 1) * pageSize, pageSize);
            MySqlParameter[] parameters = {
                        new MySqlParameter("@currentpage", MySqlDbType.Int16),
                        new MySqlParameter("@pagesize", MySqlDbType.Int16)
                        };
            parameters[0].Value = currentPage;
            parameters[1].Value = pageSize;
            return DbHelperSQL.Query(strSql.ToString(), parameters);
        }

        public int DeleteBasicInfo(int ID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat(" delete from paramoption where ID ='{0}' ", ID);
            return DbHelperSQL.ExecuteSql(strSql.ToString());
        }

        public int AddBasicInfo(string basicType, string name)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine(" insert into paramoption values ");
            strSql.AppendFormat(" (null,'{0}','{1}','{2}' ) ", basicType, name, name);
            return DbHelperSQL.ExecuteSql(strSql.ToString());
        }

        public bool ExistsBasicInfo(string basicType, string name)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat(" select count(1) from paramoption where ParamType='{0}' and ParamOptionCode='{1}' ", basicType, name);
            return DbHelperSQL.Exists(strSql.ToString());
        }
        #endregion

        #region 岗位信息
        public DataSet GetPostInfoList(int currentPage, int pageSize, string strWhere, string filedOrder)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine("  select COUNT(1) as totalrow from postinfo;  ");
            strSql.AppendLine(" select a.* from postinfo a ");
            strSql.AppendFormat(" where 1=1 {0} ", strWhere);
            if (filedOrder.Trim() != "")
            {
                strSql.Append(" order by " + filedOrder);
            }
            strSql.AppendFormat(" LIMIT {0},{1} ", (currentPage - 1) * pageSize, pageSize);
            MySqlParameter[] parameters = {
                        new MySqlParameter("@currentpage", MySqlDbType.Int16),
                        new MySqlParameter("@pagesize", MySqlDbType.Int16)
                        };
            parameters[0].Value = currentPage;
            parameters[1].Value = pageSize;
            return DbHelperSQL.Query(strSql.ToString(), parameters);
        }

        public int AddPostInfo(PostInfo model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine(" insert into postinfo(ID,PostLevel,PostName,Remark,Rate) values ");
            strSql.AppendFormat(" (null,'{0}','{1}','{2}','{3}') ", model.PostLevel, model.PostName, model.Remark, model.Rate);
            return DbHelperSQL.ExecuteSql(strSql.ToString());
        }

        public int UpdatePostInfo(PostInfo model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine(" update postinfo ");
            strSql.AppendFormat("  set PostLevel='{0}',PostName='{1}',Remark='{2}',Rate='{3}' ", model.PostLevel, model.PostName, model.Remark, model.Rate);
            strSql.AppendFormat(" where ID='{0}' ", model.ID);
            return DbHelperSQL.ExecuteSql(strSql.ToString());
        }
        public PostInfo GetPostInfoByID(int ID)
        {
            PostInfo model = new PostInfo();
            string strSql = string.Format(" select * from postinfo where ID ='{0}' ", ID);
            DataSet ds = new DataSet();
            ds = DbHelperSQL.Query(strSql);
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var row = ds.Tables[0].Rows[0];
                model.ID = ID;
                model.PostLevel = row["PostLevel"].ToString();
                model.PostName = row["PostName"].ToString();
                model.Remark = row["Remark"].ToString();
                model.Rate = (row["Rate"] == DBNull.Value) ? 0 : Convert.ToDecimal(row["Rate"]);
            }
            return model;
        }
        public bool ExistsPostLevel(PostInfo model)
        {
            StringBuilder strSql = new StringBuilder();
            if (model.ID > 0)
            {
                strSql.AppendFormat(" select count(1) from postinfo where PostLevel='{0}' and ID <>'{1}' ", model.PostLevel, model.ID);
            }
            else
            {
                strSql.AppendFormat(" select count(1) from postinfo where PostLevel='{0}' ", model.PostLevel);
            }
            return DbHelperSQL.Exists(strSql.ToString());
        }
        public bool ExistsPostName(PostInfo model)
        {
            StringBuilder strSql = new StringBuilder();
            if (model.ID > 0)
            {
                strSql.AppendFormat(" select count(1) from postinfo where PostName='{0}' and ID <>'{1}' ", model.PostName, model.ID);
            }
            else
            {
                strSql.AppendFormat(" select count(1) from postinfo where PostName='{0}' ", model.PostName);
            }
            return DbHelperSQL.Exists(strSql.ToString());
        }
        #endregion

        #region 合理性检测
        //连续两天的班次时间间隔小于12小时
        public DataSet GetReasonableSchedualInfo(string type, DateTime beginTime, DateTime endTime,string typeConfig)
        {
            DataSet ds = new DataSet();
            StringBuilder strSql = new StringBuilder();
            StringBuilder sqlWhere = new StringBuilder();
            if (type == "a")            //连续两天的班次时间间隔小于12小时
            {
                sqlWhere.AppendFormat(" and a.DutyDate>= '{0}' ", beginTime.ToString("yyyy-MM-dd"));
                sqlWhere.AppendFormat(" and a.DutyDate<= '{0}' ", endTime.ToString("yyyy-MM-dd"));

                strSql.AppendLine(" select tempa.*,tempb.`Name` from (  ");
                strSql.AppendLine(" select a.StaffNo,a.DutyDate,a.Schedual,a.BeginTime,a.EndTime,(select b.EndTime from schedualinfo b where b.StaffNo=a.StaffNo and b.DutyDate<a.DutyDate order by b.DutyDate desc LIMIT 0,1 ) as LastEndTime ");
                strSql.AppendFormat(" from schedualinfo a where 1=1 {0} ", sqlWhere);
                strSql.AppendFormat(" ) tempa inner join staffinfo tempb on tempa.StaffNo=tempb.StaffNo  where TIMESTAMPDIFF(MINUTE,tempa.LastEndTime,tempa.BeginTime)<{0} ", Convert.ToInt16(typeConfig) * 60);
            }
            else if (type == "b")            //连续上班的提醒天数超过6天
            {
                sqlWhere.AppendFormat(" and a.DutyDate>= '{0}' ", beginTime.AddDays(-Convert.ToInt32(typeConfig)).ToString("yyyy-MM-dd"));
                sqlWhere.AppendFormat(" and a.DutyDate<= '{0}' ", endTime.ToString("yyyy-MM-dd"));

                strSql.AppendLine(" select a.*,b.`Name` from ( ");
                strSql.AppendLine(" select tempa.StaffNo,MIN(tempa.DutyDate) as BeginDate,MAX(tempa.DutyDate) as EndDate,COUNT(1) as Days from ( ");
                strSql.AppendLine(" select a.StaffNo,a.DutyDate,a.Schedual,a.BeginTime,a.EndTime, ");
                strSql.AppendLine(" (select COUNT(1) from schedualinfo b  ");
                strSql.AppendLine(" 								 where b.StaffNo=a.StaffNo and b.DutyDate<a.DutyDate and b.Hours>0 ) as nums ");
                strSql.AppendFormat(" from schedualinfo a where 1=1 and a.Hours>0 {0} ", sqlWhere);
                strSql.AppendLine(" ) tempa ");
                strSql.AppendLine(" GROUP BY tempa.StaffNo,DATE_ADD(tempa.DutyDate,INTERVAL -tempa.nums DAY) ");
                strSql.AppendLine(" ) a inner join staffinfo b on a.StaffNo=b.StaffNo ");
                strSql.AppendFormat(" where a.Days>{0}  ", Convert.ToInt16(typeConfig));
            }
            else if (type == "c")       //连续夜班的提醒天数超过2天
            {
                sqlWhere.AppendFormat(" and a.DutyDate>= '{0}' ", beginTime.AddDays(-Convert.ToInt32(typeConfig)).ToString("yyyy-MM-dd"));
                sqlWhere.AppendFormat(" and a.DutyDate<= '{0}' ", endTime.ToString("yyyy-MM-dd"));

                strSql.AppendLine(" select a.*,b.`Name` from ( ");
                strSql.AppendLine(" select tempa.StaffNo,MIN(tempa.DutyDate) as BeginDate,MAX(tempa.DutyDate) as EndDate,COUNT(1) as Days from ( ");
                strSql.AppendLine(" select a.StaffNo,a.DutyDate,a.Schedual,a.BeginTime,a.EndTime, ");
                strSql.AppendLine(" (select COUNT(1) from schedualinfo b  ");
                strSql.AppendLine(" 								 where b.StaffNo=a.StaffNo and b.DutyDate<a.DutyDate and b.Hours>0 and b.IsNight=1 ) as nums ");
                strSql.AppendFormat(" from schedualinfo a where 1=1 and a.Hours>0 and a.IsNight=1 {0} ", sqlWhere);
                strSql.AppendLine(" ) tempa ");
                strSql.AppendLine(" GROUP BY tempa.StaffNo,DATE_ADD(tempa.DutyDate,INTERVAL -tempa.nums DAY) ");
                strSql.AppendLine(" ) a inner join staffinfo b on a.StaffNo=b.StaffNo ");
                strSql.AppendFormat(" where a.Days>{0}  ", Convert.ToInt16(typeConfig));
            }
            else if (type == "d")       //整月班表中单个员工最大工时和最小工时差异小时数
            {
                //先获取最小工时数
                strSql.AppendLine(" select tempc.StaffNo,tempc.ActSumHours as MinSumHours from ( ");
                strSql.AppendLine(" select tempa.*,FORMAT((tempa.SumHours/(31-(tempb.LeaveDays+tempb.OutDays))*31),1) as ActSumHours  ");
                strSql.AppendLine(" from (select a.StaffNo,SUM(a.Hours) as SumHours from schedualinfo a ");
                strSql.AppendFormat(" where a.DutyDate>='{0}' and a.DutyDate<='{1}' ", beginTime, endTime);
                strSql.AppendLine(" GROUP BY a.StaffNo) tempa ");
                strSql.AppendFormat(" left join (select a.StaffNo,(CASE WHEN(a.HireDate>'{0}') THEN DATEDIFF(a.HireDate,'{0}') else 0 END) as OutDays  ", beginTime);
                strSql.AppendLine(" 						,IFNULL(DATEDIFF(b.EndDate,b.BeginDate)+1,0) as LeaveDays from staffinfo a ");
                strSql.AppendLine(" 						left join staffleave b on a.StaffNo=b.StaffNo ");
                strSql.AppendLine(" ) tempb on tempa.StaffNo=tempb.StaffNo) tempc order by tempc.ActSumHours LIMIT 0,1 ");
                DataSet dsTemp = new DataSet();
                dsTemp = DbHelperSQL.Query(strSql.ToString());
                decimal minSumHours = 0;
                decimal actMinSumHours = 0;
                string minStaffName = string.Empty;
                if (dsTemp != null && dsTemp.Tables.Count > 0 && dsTemp.Tables[0].Rows.Count > 0)
                {
                    minSumHours = Convert.ToDecimal(dsTemp.Tables[0].Rows[0]["MinSumHours"]);
                    actMinSumHours = minSumHours + Convert.ToInt16(typeConfig);
                    minStaffName = GetStaffNameByStaffNo(dsTemp.Tables[0].Rows[0]["StaffNo"].ToString());
                }
                strSql = new StringBuilder();
                strSql.AppendLine(" select '" + minSumHours + "' as MinSumHours,'" + minStaffName + "' as MinStaffName,tempc.*,tempd.`Name` from ( ");
                strSql.AppendLine(" select tempa.*,FORMAT((tempa.SumHours/(31-(tempb.LeaveDays+tempb.OutDays))*31),1) as ActSumHours  ");
                strSql.AppendLine(" from (select a.StaffNo,SUM(a.Hours) as SumHours from schedualinfo a ");
                strSql.AppendFormat(" where a.DutyDate>='{0}' and a.DutyDate<='{1}' ", beginTime, endTime);
                strSql.AppendLine(" GROUP BY a.StaffNo) tempa ");
                strSql.AppendFormat(" left join (select a.StaffNo,(CASE WHEN(a.HireDate>'{0}') THEN DATEDIFF(a.HireDate,'{0}') else 0 END) as OutDays  ", beginTime);
                strSql.AppendLine(" 						,IFNULL(DATEDIFF(b.EndDate,b.BeginDate)+1,0) as LeaveDays from staffinfo a ");
                strSql.AppendLine(" 						left join staffleave b on a.StaffNo=b.StaffNo ");
                strSql.AppendLine(" ) tempb on tempa.StaffNo=tempb.StaffNo) tempc ");
                strSql.AppendLine(" inner join staffinfo tempd on tempc.StaffNo=tempd.StaffNo ");
                strSql.AppendFormat(" where tempc.ActSumHours>{0} ", actMinSumHours);
            }
            else if (type == "e")       //整月班表中单个员工夜班数量差异个数
            {
                //先获取最小工时数
                strSql.AppendLine(" select tempc.StaffNo,tempc.ActSumNights as MinSumNights from ( ");
                strSql.AppendLine(" select tempa.*,FORMAT((tempa.SumNights/(31-(tempb.LeaveDays+tempb.OutDays))*31),1) as ActSumNights ");
                strSql.AppendLine(" from (select a.StaffNo,COUNT(1) as SumNights from schedualinfo a ");
                strSql.AppendFormat(" where a.IsNight=1 and a.DutyDate>='{0}' and a.DutyDate<='{1}' ", beginTime, endTime);
                strSql.AppendLine(" GROUP BY a.StaffNo) tempa ");
                strSql.AppendFormat(" left join (select a.StaffNo,(CASE WHEN(a.HireDate>'{0}') THEN DATEDIFF(a.HireDate,'{0}') else 0 END) as OutDays  ", beginTime);
                strSql.AppendLine(" 						,IFNULL(DATEDIFF(b.EndDate,b.BeginDate)+1,0) as LeaveDays from staffinfo a ");
                strSql.AppendLine(" 						left join staffleave b on a.StaffNo=b.StaffNo ");
                strSql.AppendLine(" ) tempb on tempa.StaffNo=tempb.StaffNo) tempc order by tempc.ActSumNights LIMIT 0,1 ");
                DataSet dsTemp = new DataSet();
                dsTemp = DbHelperSQL.Query(strSql.ToString());
                decimal minSumNights = 0;
                decimal actMinSumNights = 0;
                string minStaffName = string.Empty;
                if (dsTemp != null && dsTemp.Tables.Count > 0 && dsTemp.Tables[0].Rows.Count > 0)
                {
                    minSumNights = Convert.ToDecimal(dsTemp.Tables[0].Rows[0]["MinSumNights"]);
                    actMinSumNights = minSumNights + Convert.ToInt16(typeConfig);
                    minStaffName = GetStaffNameByStaffNo(dsTemp.Tables[0].Rows[0]["StaffNo"].ToString());
                }
                strSql = new StringBuilder();
                strSql.AppendLine(" select '" + minSumNights + "' as MinSumNights,'" + minStaffName + "' as MinStaffName,tempc.*,tempd.`Name` from ( ");
                strSql.AppendLine(" select tempa.*,FORMAT((tempa.SumNights/(31-(tempb.LeaveDays+tempb.OutDays))*31),1) as ActSumNights ");
                strSql.AppendLine(" from (select a.StaffNo,COUNT(1) as SumNights from schedualinfo a ");
                strSql.AppendFormat(" where a.IsNight=1 and a.DutyDate>='{0}' and a.DutyDate<='{1}' ", beginTime, endTime);
                strSql.AppendLine(" GROUP BY a.StaffNo) tempa ");
                strSql.AppendFormat(" left join (select a.StaffNo,(CASE WHEN(a.HireDate>'{0}') THEN DATEDIFF(a.HireDate,'{0}') else 0 END) as OutDays  ", beginTime);
                strSql.AppendLine(" 						,IFNULL(DATEDIFF(b.EndDate,b.BeginDate)+1,0) as LeaveDays from staffinfo a ");
                strSql.AppendLine(" 						left join staffleave b on a.StaffNo=b.StaffNo ");
                strSql.AppendLine(" ) tempb on tempa.StaffNo=tempb.StaffNo) tempc  ");
                strSql.AppendLine(" inner join staffinfo tempd on tempc.StaffNo=tempd.StaffNo ");
                strSql.AppendFormat(" where tempc.ActSumNights>{0} ", actMinSumNights);
            }
            ds = DbHelperSQL.Query(strSql.ToString());
            return ds;
        }

        public int SaveReasonableParam(ReasonableParam model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat(" update paramoption set ParamOptionCode='{0}',ParamOptionName='{0}' where ParamType='ReasonableConfig_A'; ", model.ParamType_A);
            strSql.AppendFormat(" update paramoption set ParamOptionCode='{0}',ParamOptionName='{0}' where ParamType='ReasonableConfig_B'; ", model.ParamType_B);
            strSql.AppendFormat(" update paramoption set ParamOptionCode='{0}',ParamOptionName='{0}' where ParamType='ReasonableConfig_C'; ", model.ParamType_C);
            strSql.AppendFormat(" update paramoption set ParamOptionCode='{0}',ParamOptionName='{0}' where ParamType='ReasonableConfig_D'; ", model.ParamType_D);
            strSql.AppendFormat(" update paramoption set ParamOptionCode='{0}',ParamOptionName='{0}' where ParamType='ReasonableConfig_E'; ", model.ParamType_E);
            return DbHelperSQL.ExecuteSql(strSql.ToString());
        }

        public ReasonableParam GetReasonableParam()
        {
            ReasonableParam model = new ReasonableParam();
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine(" select * from paramoption where ParamType like '%ReasonableConfig%' ");
            DataSet ds = new DataSet();
            ds = DbHelperSQL.Query(strSql.ToString());
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    var row = ds.Tables[0].Rows[i];
                    if (row["ParamType"].ToString() == "ReasonableConfig_A")
                    {
                        model.ParamType_A = row["ParamOptionCode"].ToString();
                    }
                    if (row["ParamType"].ToString() == "ReasonableConfig_B")
                    {
                        model.ParamType_B = row["ParamOptionCode"].ToString();
                    }
                    if (row["ParamType"].ToString() == "ReasonableConfig_C")
                    {
                        model.ParamType_C = row["ParamOptionCode"].ToString();
                    }
                    if (row["ParamType"].ToString() == "ReasonableConfig_D")
                    {
                        model.ParamType_D = row["ParamOptionCode"].ToString();
                    }
                    if (row["ParamType"].ToString() == "ReasonableConfig_E")
                    {
                        model.ParamType_E = row["ParamOptionCode"].ToString();
                    }
                }
            }
            return model;
        }
        #endregion

        #region 员工请假
        public DataSet GetStaffLeaveList(int currentPage, int pageSize, string strWhere, string filedOrder)
        {
            //select * from table limit (start-1)*limit,limit; 其中start是页码，limit是每页显示的条数。
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine("  select COUNT(1) as totalrow from staffleave;  ");
            strSql.AppendLine(" select a.*,b.Name from staffleave a inner join staffinfo b on a.StaffNo = b.StaffNo ");
            strSql.AppendFormat(" where 1=1 {0} ", strWhere);
            if (filedOrder.Trim() != "")
            {
                strSql.Append(" order by " + filedOrder);
            }
            strSql.AppendFormat(" LIMIT {0},{1} ", (currentPage - 1) * pageSize, pageSize);
            MySqlParameter[] parameters = {
                        new MySqlParameter("@currentpage", MySqlDbType.Int16),
                        new MySqlParameter("@pagesize", MySqlDbType.Int16)
                        };
            parameters[0].Value = currentPage;
            parameters[1].Value = pageSize;
            return DbHelperSQL.Query(strSql.ToString(), parameters);
        }

        public int AddStaffLeave(StaffLeave model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine(" insert into staffleave(ID,StaffNo,LeaveType,BeginDate,EndDate,CreateOn,Status,Remark) values ");
            strSql.AppendFormat(" (null,'{0}','{1}','{2}','{3}',now(),'{4}','{5}' ) ",
                model.StaffNo, model.LeaveType, model.BeginDate, model.EndDate, model.Status, model.Remark);
            return DbHelperSQL.ExecuteSql(strSql.ToString());
        }

        public int UpdateStaffLeave(StaffLeave model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine(" update staffleave ");
            strSql.AppendFormat("  set StaffNo='{0}',LeaveType='{1}',BeginDate='{2}',EndDate='{3}',Status='{4}',Remark='{5}' ",
                model.StaffNo, model.LeaveType, model.BeginDate, model.EndDate, model.Status, model.Remark);
            strSql.AppendFormat(" where ID='{0}' ", model.ID);
            return DbHelperSQL.ExecuteSql(strSql.ToString());
        }
        public StaffLeave GetStaffLeaveByID(int ID)
        {
            StaffLeave model = new StaffLeave();
            string strSql = string.Format(" select * from staffleave where ID ='{0}' ", ID);
            DataSet ds = new DataSet();
            ds = DbHelperSQL.Query(strSql);
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var row = ds.Tables[0].Rows[0];
                model.ID = ID;
                model.StaffNo = row["StaffNo"].ToString();
                model.LeaveType = row["LeaveType"].ToString();
                model.CreateOn = Convert.ToDateTime(row["CreateOn"]);
                model.Status = Convert.ToInt16(row["Status"]);
                model.Remark = row["Remark"].ToString();
                model.BeginDate = Convert.ToDateTime(row["BeginDate"]);
                model.EndDate = Convert.ToDateTime(row["EndDate"]);
            }
            return model;
        }
        #endregion

        #region 员工人脉
        public DataSet GetStaffConnectionList(int currentPage, int pageSize, string strWhere, string filedOrder)
        {
            //select * from table limit (start-1)*limit,limit; 其中start是页码，limit是每页显示的条数。
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine("  select COUNT(1) as totalrow from staffconnection;  ");
            strSql.AppendLine(" select a.*,b.Name as StaffName,c.Name as ConnectStaffName,d.Name as ConnectGuestName from staffconnection a  ");
            strSql.AppendLine(" inner join staffinfo b on a.StaffNo = b.StaffNo ");
            strSql.AppendLine(" inner join staffinfo c on a.ConnectStaffNo=c.StaffNo ");
            strSql.AppendLine(" inner join guestinfo d on a.ConnectGuestID = d.ID ");
            strSql.AppendFormat(" where 1=1 {0} ", strWhere);
            if (filedOrder.Trim() != "")
            {
                strSql.Append(" order by " + filedOrder);
            }
            strSql.AppendFormat(" LIMIT {0},{1} ", (currentPage - 1) * pageSize, pageSize);
            MySqlParameter[] parameters = {
                        new MySqlParameter("@currentpage", MySqlDbType.Int16),
                        new MySqlParameter("@pagesize", MySqlDbType.Int16)
                        };
            parameters[0].Value = currentPage;
            parameters[1].Value = pageSize;
            return DbHelperSQL.Query(strSql.ToString(), parameters);
        }

        public int AddStaffConnection(StaffConnection model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine(" insert into staffconnection(ID,StaffNo,ConnectStaffNo,ConnectGuestID,OtherConnectName,ConnectionType,ModifyOn,Remark) values ");
            strSql.AppendFormat(" (null,'{0}','{1}','{2}','{3}','{4}',now(),'{5}' ) ",
                model.StaffNo, model.ConnectStaffNo, model.ConnectGuestID, model.OtherConnectName, model.ConnectionType, model.Remark);
            return DbHelperSQL.ExecuteSql(strSql.ToString());
        }

        public int UpdateStaffConnection(StaffConnection model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine(" update staffconnection ");
            strSql.AppendFormat("  set StaffNo='{0}',ConnectStaffNo='{1}',ConnectGuestID='{2}',OtherConnectName='{3}',ConnectionType='{4}',Remark='{5}',ModifyOn=now() ",
                model.StaffNo, model.ConnectStaffNo, model.ConnectGuestID, model.OtherConnectName, model.ConnectionType, model.Remark);
            strSql.AppendFormat(" where ID='{0}' ", model.ID);
            return DbHelperSQL.ExecuteSql(strSql.ToString());
        }
        public StaffConnection GetStaffConnectionByID(int ID)
        {
            StaffConnection model = new StaffConnection();
            string strSql = string.Format(" select * from staffconnection where ID ='{0}' ", ID);
            DataSet ds = new DataSet();
            ds = DbHelperSQL.Query(strSql);
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var row = ds.Tables[0].Rows[0];
                model.ID = ID;
                model.StaffNo = row["StaffNo"].ToString();
                model.ConnectStaffNo = row["ConnectStaffNo"].ToString();
                model.ConnectGuestID = row["ConnectGuestID"].ToString();
                model.OtherConnectName = row["OtherConnectName"].ToString();
                model.Remark = row["Remark"].ToString();
                model.ConnectionType = row["ConnectionType"].ToString();
                model.ModifyOn = Convert.ToDateTime(row["ModifyOn"]);
            }
            return model;
        }
        public int DeleteStaffConnection(int ID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat(" delete from staffconnection where ID ='{0}' ", ID);
            return DbHelperSQL.ExecuteSql(strSql.ToString());
        }
        #endregion

        #region 楼层信息
        public DataSet GetFloorList(int currentPage, int pageSize, string strWhere, string filedOrder)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine("  select COUNT(1) as totalrow from floor;  ");
            strSql.AppendLine(" select a.*,b.Name from floor a left join staffinfo b on a.Manager=b.StaffNo ");
            strSql.AppendFormat(" where 1=1 {0} ", strWhere);
            if (filedOrder.Trim() != "")
            {
                strSql.Append(" order by " + filedOrder);
            }
            strSql.AppendFormat(" LIMIT {0},{1} ", (currentPage - 1) * pageSize, pageSize);
            MySqlParameter[] parameters = {
                        new MySqlParameter("@currentpage", MySqlDbType.Int16),
                        new MySqlParameter("@pagesize", MySqlDbType.Int16)
                        };
            parameters[0].Value = currentPage;
            parameters[1].Value = pageSize;
            return DbHelperSQL.Query(strSql.ToString(), parameters);
        }

        public int AddFloor(Floor model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine(" insert into floor(ID,FloorID,Remark,Manager,Status) values ");
            strSql.AppendFormat(" (null,'{0}','{1}','{2}','{3}') ", model.FloorID, model.Remark, model.Manager, model.Status);
            return DbHelperSQL.ExecuteSql(strSql.ToString());
        }

        public int UpdateFloor(Floor model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine(" update floor ");
            strSql.AppendFormat("  set FloorID='{0}',Remark='{1}',Manager='{2}',Status='{3}' ", model.FloorID,model.Remark, model.Manager, model.Status);
            strSql.AppendFormat(" where ID='{0}' ", model.ID);
            return DbHelperSQL.ExecuteSql(strSql.ToString());
        }
        public Floor GetFloorByID(int ID)
        {
            Floor model = new Floor();
            string strSql = string.Format(" select * from floor where ID ='{0}' ", ID);
            DataSet ds = new DataSet();
            ds = DbHelperSQL.Query(strSql);
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var row = ds.Tables[0].Rows[0];
                model.ID = ID;
                model.FloorID = row["FloorID"].ToString();
                model.Remark = row["Remark"].ToString();
                model.Manager = row["Manager"].ToString();
                model.Status = Convert.ToInt16(row["Status"]);
            }
            return model;
        }
        public bool ExistsFloor(Floor model)
        {
            StringBuilder strSql = new StringBuilder();
            if (model.ID > 0)
            {
                strSql.AppendFormat(" select count(1) from floor where FloorID='{0}' and ID <>'{1}' ", model.FloorID, model.ID);
            }
            else
            {
                strSql.AppendFormat(" select count(1) from floor where FloorID='{0}' ", model.FloorID);
            }
            return DbHelperSQL.Exists(strSql.ToString());
        }

        public DataSet GetFloorInfo()
        {
            DataSet ds = new DataSet();
            string strSql = "select * from floor";
            ds = DbHelperSQL.Query(strSql);
            return ds;
        }
        #endregion

        #region 房间信息
        public DataSet GetRoomList(int currentPage, int pageSize, string strWhere, string filedOrder)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine("  select COUNT(1) as totalrow from room;  ");
            strSql.AppendLine(" select a.* from room a ");
            strSql.AppendFormat(" where 1=1 {0} ", strWhere);
            if (filedOrder.Trim() != "")
            {
                strSql.Append(" order by " + filedOrder);
            }
            strSql.AppendFormat(" LIMIT {0},{1} ", (currentPage - 1) * pageSize, pageSize);
            MySqlParameter[] parameters = {
                        new MySqlParameter("@currentpage", MySqlDbType.Int16),
                        new MySqlParameter("@pagesize", MySqlDbType.Int16)
                        };
            parameters[0].Value = currentPage;
            parameters[1].Value = pageSize;
            return DbHelperSQL.Query(strSql.ToString(), parameters);
        }

        public int AddRoom(Room model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine(" insert into room(ID,RoomNo,FloorID,RoomSex,Remark,PeopleNum,Status) values ");
            strSql.AppendFormat(" (null,'{0}','{1}','{2}','{3}','{4}','{5}') ", model.RoomNo, model.FloorID, model.RoomSex, model.Remark, model.PeopleNum, model.Status);
            return DbHelperSQL.ExecuteSql(strSql.ToString());
        }

        public int UpdateRoom(Room model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine(" update room ");
            strSql.AppendFormat("  set RoomNo='{0}',FloorID='{1}',RoomSex='{2}',Remark='{3}',PeopleNum='{4}',Status='{5}' ", model.RoomNo, model.FloorID, model.RoomSex, model.Remark, model.PeopleNum, model.Status);
            strSql.AppendFormat(" where ID='{0}' ", model.ID);
            return DbHelperSQL.ExecuteSql(strSql.ToString());
        }
        public Room GetRoomByID(int ID)
        {
            Room model = new Room();
            string strSql = string.Format(" select * from room where ID ='{0}' ", ID);
            DataSet ds = new DataSet();
            ds = DbHelperSQL.Query(strSql);
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var row = ds.Tables[0].Rows[0];
                model.ID = ID;
                model.RoomNo = row["RoomNo"].ToString();
                model.FloorID = row["FloorID"].ToString();
                model.Remark = row["Remark"].ToString();
                model.RoomSex = Convert.ToInt16(row["RoomSex"]);
                model.PeopleNum = Convert.ToInt16(row["PeopleNum"]);
                model.Status = Convert.ToInt16(row["Status"]);
            }
            return model;
        }
        public bool ExistsRoom(Room model)
        {
            StringBuilder strSql = new StringBuilder();
            if (model.ID > 0)
            {
                strSql.AppendFormat(" select count(1) from room where RoomNo='{0}' and ID <>'{1}' ", model.RoomNo, model.ID);
            }
            else
            {
                strSql.AppendFormat(" select count(1) from room where RoomNo='{0}' ", model.RoomNo);
            }
            return DbHelperSQL.Exists(strSql.ToString());
        }

        public DataSet GetRoomInfo()
        {
            DataSet ds = new DataSet();
            string strSql = "select * from room";
            ds = DbHelperSQL.Query(strSql);
            return ds;
        }
        #endregion

    }
}