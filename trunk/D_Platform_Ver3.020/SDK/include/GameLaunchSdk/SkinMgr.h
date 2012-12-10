#ifndef WM_EXCHANGE_SKIN
#define WM_EXCHANGE_SKIN			WM_USER + 1666				//����Ƥ��
#endif

#ifndef	__SKINMGR__H__
#define	__SKINMGR__H__

#include "StdAfx.h"

class EXT_CLASS CSkinMgr
{
public:
	CSkinMgr();
	~CSkinMgr();

public:

	CString GetValOfSkinBcf(const CString& strLogonUserID = "");

public:

	CString GetUserSelSkinIndex() const;

	//��ȡƤ��·��
	CString GetSkinPath(const CString& strLogonUserID = "");

	//��ȡ��Ϸ����Ƥ��·��
	CString GetGameSkinPath();

	//��ȡƤ��·���µĶ�Ӧ�ļ���
	CString GetSkinBcfFileName(const CString& strLogonUserID = "");

	//��ȡ��Ϸ����·����Ӧ���ļ���
	CString GetGameSkinBcfFileName(const CString& strLogonUserID = "");

	//���õ�ǰ�û�ѡ�е�Ƥ��
	void SetCurUserSelSkin(const CString& strindex = "");

	//��ȡkeyֵ
	CString GetKeyVal(const CString& strSkinBcfFileName);

	//֪ͨ����ˢ��Ƥ��
	void Notify();

	//����ʱ�������õ��û���
	void SetLogOnUserName(const CString& strLogonName);

	//���û��Ƿ��һ�ε�¼
	bool IsUserCountFirstLogOn(const CString& strLogonUserID);

private:

	//���õ�Ƥ��ѡ������
	static CString m_skinIndex;

	//�û���
	static CString m_strUserName;

};



#endif