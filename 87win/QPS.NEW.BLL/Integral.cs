﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using QPS.NEW.DAL;

namespace QPS.NEW.BLL
{
    public class Integral
    {
        private SQLHelper sqlHelper_;

        public Integral()
        {
            sqlHelper_ = new SQLHelper(null);
        }

        public QPS.NEW.Model.Integral GetModel(int Id)
        {
            QPS.NEW.Model.Integral integral = null;

            string strSql = "select UserID,WalletMoney,BankMoney,WalletMoney+BankMoney as TotalMoney from TUserInfo where UserID=@id";
            DataTable dt = sqlHelper_.GetDataTable(
                strSql,
                CommandType.Text,
                new SqlParameter[]
                {
                    new SqlParameter("@id",Id)
                }
                );

            if (dt != null && dt.Rows.Count > 0)
            {
                integral = new Model.Integral();

                integral.UserID =Convert.ToInt32( dt.Rows[0]["UserID"] );
                integral.BankMoney = Convert.ToInt32(dt.Rows[0]["BankMoney"]);
                integral.WalletMoney = Convert.ToInt32(dt.Rows[0]["WalletMoney"]);
                integral.TotalMoney = Convert.ToInt32(dt.Rows[0]["TotalMoney"]);
            }
            return integral;
        }

        public bool Update(QPS.NEW.Model.Integral model)
        {
            bool res = false;

            string[] filedName = new string[50];
            string[] paramName = new string[50];
            SqlParameter[] sqlParams = new SqlParameter[50];
            int Count = 0;


            if (model.UserID == -999 && model.TotalMoney!=-999)
                return false;

            if (model.UserID != -999)
            {
                filedName[Count] = "UserID";
                paramName[Count] = "@" + filedName[Count];
                sqlParams[Count] = new SqlParameter(paramName[Count], model.UserID);
                Count++;
            }
            int newWalletMoney, newBankMoney;
            int bankMoney = GetBankMoney(model.UserID);
            int walletMoney = GetWalletMoney(model.UserID);

            if (bankMoney + walletMoney < model.TotalMoney) //增加积分,直接写增加金额到Bank
            {
                newWalletMoney = walletMoney;
                newBankMoney = model.TotalMoney - walletMoney;
            }
            else // 减少积分
            {
                int reducedMoney = model.TotalMoney - (bankMoney + walletMoney);
                if (bankMoney >= reducedMoney) // 优先操作Bank
                {
                    newBankMoney = bankMoney - reducedMoney;
                    newWalletMoney = walletMoney;
                }
                else
                {
                    newBankMoney = 0;
                    newWalletMoney = walletMoney + bankMoney - reducedMoney;
                }
            }


                filedName[Count] = "WalletMoney";
                paramName[Count] = "@" + filedName[Count];
                sqlParams[Count] = new SqlParameter(paramName[Count], newWalletMoney);
                Count++;


                filedName[Count] = "BankMoney";
                paramName[Count] = "@" + filedName[Count];
                sqlParams[Count] = new SqlParameter(paramName[Count], newBankMoney);
                Count++;


            StringBuilder strSql = new StringBuilder();
            strSql.Append("update TUserInfo set ");
            for (int i = 1; i < Count; i++)
            {
                strSql.Append(filedName[i]);
                strSql.Append("=");
                strSql.Append(paramName[i]);
                if (i != Count - 1)
                {
                    strSql.Append(",");
                }
            }
            strSql.Append(" where ");
            strSql.Append(filedName[0] + "=" + paramName[0]);


            int num = Convert.ToInt32(sqlHelper_.ExecuteCommand(
                strSql.ToString(),
                CommandType.Text,
                sqlParams
                ));

            if (num != 1)
            {
                res = false;
            }
            else
            {
                res = true;
            }

            return res;

        }

        public DataSet GetListlocalize(string strWhere)
        {
            DataSet ds = null;

            string strSql = "select UserID as 用户ID,WalletMoney/10000 as 钱包中的积分,BankMoney/10000 as 银行中的积分 from TUserInfo where " + strWhere;
            ds = sqlHelper_.GetDataSet(strSql, CommandType.Text, null);

            return ds;
        }

        public DataSet GetSum(int userId)
        {
            DataSet ds = null;
            string strSql = "select (WalletMoney+BankMoney)/10000 from TUserInfo where UserID=@UserID";

            ds = sqlHelper_.GetDataSet(strSql, CommandType.Text,
                new SqlParameter[]{
                    new SqlParameter("@UserID",userId)
                }
                );

            return ds;
        }

        public DataSet GetList(string strWhere)
        {
            DataSet ds = null;

            ds = sqlHelper_.GetDataSet("select UserID,(WalletMoney+BankMoney)/10000 as TotalMoney from TUserInfo where " + strWhere,
                CommandType.Text,
                null);
            return ds;
        }

        public bool Delete(int Id)
        {
            bool res = false;

            int num = sqlHelper_.ExecuteCommand(
                "update TUserInfo set WalletMoney=0,BankMoney=0 where UserID=@userid",
                                CommandType.Text,
                new SqlParameter[]
                {
                    new SqlParameter("@userid",Id)
                }
                );

            if (num == 1)
                res = true;

            return res;
        }

        public bool DeleteByUid(int userId)
        {

            return Delete(userId);
        }

        public int Add(QPS.NEW.Model.Integral model)
        {
            int bankMoney = GetBankMoney(model.UserID) + model.BankMoney;
            int num = sqlHelper_.ExecuteCommand(
                "update TUserInfo set BankMoney=@bankmoney",
                CommandType.Text,
                new SqlParameter("@bankmoney",bankMoney)
                );

            if (num==1)
                return 1;
            else
                return 0;
        }

        public DataSet Select(int pageSize, int currentPage)
        {
            int hasShowedPage = 0;

            hasShowedPage = currentPage - 1 >= 0 ? currentPage - 1 : 0;


            string strSql = "select ui.UserID,(ui.WalletMoney+ui.BankMoney)/10000 as TotalMoney,u.UserName from TUserInfo as ui,TUsers as u where ui.UserID=u.UserID";

            DataSet ds = null;
            ds = sqlHelper_.GetDataSet(
                strSql,
                CommandType.Text,
                null
                );

            return ds;
        }

        public int GetCount()
        {
            int res = -999;

            res = Convert.ToInt32(
                sqlHelper_.GetSingle(
                "select count(*) from TUserInfo",
                CommandType.Text,
                null
                )
                );

            return res;
        }

        public DataSet SelectByName(string strUsername)
        {
            DataSet ds = null;

            string strSql =
            "select u.UserID,u.UserName,(WalletMoney+BankMoney)/10000 as TotalMoney from TUserInfo as i,TUsers as u where i.UserID=u.UserID and  u.UserName=@username";
            ds = sqlHelper_.GetDataSet
                (
                strSql,
                CommandType.Text,
                new SqlParameter[]{
                    new SqlParameter("@username",strUsername)
                }
                );

            return ds;
        }

        public bool UpdateContent(QPS.NEW.Model.Integral model)
        {
            return Update(model);
        }

        public DataSet SelectByJfxq(string userid)
        {
            DataSet ds = null;

            string strSql =
            "select u.UserID,u.UserName,(WalletMoney+BankMoney)/10000 as TotalMoney from TUserInfo as i,TUsers as u where i.UserID=u.UserID and  u.UserID=@userid";
            ds = sqlHelper_.GetDataSet
                (
                strSql,
                CommandType.Text,
                new SqlParameter[]{
                    new SqlParameter("@userid",userid)
                }
                );

            return ds;
        }

        public int GetBankMoney(int userID)
        {
            int res = -999;

            res=Convert.ToInt32(
                  sqlHelper_.GetSingle(
                  "select BankMoney from TUserInfo where UserID=@UserID",
                  CommandType.Text,
                  new SqlParameter[]
                {
                    new SqlParameter("@UserID",userID)
                }
                  ));

            return res;
        }

        public int GetWalletMoney(int userID)
        {
            int res = -999;

            res = Convert.ToInt32(
                  sqlHelper_.GetSingle(
                  "select WalletMoney from TUserInfo where UserID=@UserID",
                  CommandType.Text,
                  new SqlParameter[]
                {
                    new SqlParameter("@UserID",userID)
                }
                  ));

            return res;
        }
    }
}
