
using Native.Csharp.Sdk.Cqp.EventArgs;

namespace Native.Csharp.Sdk.Cqp.Interface
{
	/// <summary>
	/// 酷Q 好友消息接口
	/// </summary>
	public interface IEvent_FriendMessage
	{
		/// <summary>
		/// Type=201 好友已添加<para/>
		/// 当在派生类中重写时, 处理好友已经添加事件
		/// </summary>
		/// <param name="sender">事件的触发对象</param>
		/// <param name="e">事件的附加参数</param>
		void ReceiveFriendIncrease (object sender, CqFriendIncreaseEventArgs e);

		/// <summary>
		/// Type=301 收到好友添加请求<para/>
		/// 当在派生类中重写时, 处理收到的好友添加请求
		/// </summary>
		/// <param name="sender">事件的触发对象</param>
		/// <param name="e">事件的附加参数</param>
		void ReceiveFriendAddRequest (object sender, CqAddFriendRequestEventArgs e);

		/// <summary>
		/// Type=21 好友消息<para/>
		/// 当在派生类中重写时, 处理收到的好友消息
		/// </summary>
		/// <param name="sender">事件的触发对象</param>
		/// <param name="e">事件的附加参数</param>
		void ReceiveFriendMessage (object sender, CqPrivateMessageEventArgs e);
	}
}
