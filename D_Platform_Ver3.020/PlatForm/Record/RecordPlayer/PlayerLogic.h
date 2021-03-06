#pragma once
#include "afxcmn.h"
#include "../include/interface.h"
#include "../include/struct.h"
//#include "../../客户端组件/界面库.2003/GameFrameDlg.h"
#include "../PRStructExchange/PRStructExchange.h"
#include "GameUserManage.h"
#include "Platform/LoveSendClassForEXE.h"
//播放器逻辑部分，由对话框部分调用
#define  MAX_PLAYER_COUNT  180


#define IDM_BIG_BROADCAST_MSG				WM_USER + 437				//大广播
#define WM_BRING_GAMEWND_TO_TOP				WM_USER + 438				//弹出游戏窗口
#define WM_COLSE_ROOM_WITH_MSG              WM_USER+219                      //关闭房间后让大厅弹出一个消息框



struct RecordNetMsgStruct
{
	BYTE NetMsgHead[MAX_MSG_HEAD_LEN]; //头消息   //最长50个字节
	BYTE NetMsgData[MAX_MSG_DATA_LEN];   //网络消息
	UINT uHeadSize;      //头的大小
	UINT uDataSize;      //网络消息大小	
	UINT uTimeSlot;      //与上一消息的时间间隔

	RecordNetMsgStruct()
	{		
		ResetMsgLen();		
	}
	void ResetMsgLen()
	{
		memset(NetMsgData,0,sizeof(NetMsgData));
		memset(NetMsgHead,0,sizeof(NetMsgHead));
		uHeadSize = MAX_MSG_HEAD_LEN;
		uDataSize = MAX_MSG_DATA_LEN;
		uTimeSlot = 0;
		
	}
};

class CPlayerLogic
{
public:
	CPlayerLogic(CWnd* pWnd);
	~CPlayerLogic(void);
     //外部调用函数，非接口调用


	//加載錄像插件
	bool OnInit();
	bool OpenFile(TCHAR* pFileName);  //打开指定的文件名
	static CString GetAppPath(BOOL bValue=false);//得到执行程序所在的路径	
	int GetNextTimeSlot();  //得到下一条消息的时间间隔
	bool PlayOneStep();  //播放一布
	bool PlayToSomeStep(UINT uStep);  //后退几步
	bool BackOneStep();//后退一步
	void InitGame();   //初始化游戏
	bool IsInBegin();  
	bool IsInEnd();
	void ClearGame();

	int GetSliderPos();
	void PlaySliderPos(int iPos);

protected:
	//啟動客戶端
	bool StartGameClient(const TCHAR* szProccName,GameInfoStruct& GameInfo);
   

	//void 


private:
	CWnd*              m_pParentWnd;  //父窗口的句柄
	IFrameInterface*   m_IGameFrame;  //游戏界面狂接		
	  
	IRecorderForPlayer* m_pRecordPlay;  //录像插件

	HINSTANCE m_hRecordInstance;
	HINSTANCE m_hGameInstance;
	CPRStructExchange m_PRStructExchange;

	GameInfoStruct m_GameInfo;
	UserItemStruct m_UserInfo[MAX_PLAYER_COUNT];

	CGameUserManage		m_UserManage;

	UINT m_uRealPlayNum;
	int m_iCurPlayStep;

	RecordNetMsgStruct m_CurMsg;



};
