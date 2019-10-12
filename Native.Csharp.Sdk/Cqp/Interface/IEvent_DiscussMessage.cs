
using Native.Csharp.Sdk.Cqp.EventArgs;

namespace Native.Csharp.Sdk.Cqp.Interface
{
	/// <summary>
	/// 酷Q 讨论组消息接口
	/// </summary>
	public interface IEvent_DiscussMessage
	{
		/// <summary>
		/// Type=4 讨论组消息 <para/>
		/// 当在派生类中重写时, 处理收到的讨论组消息
		/// </summary>
		/// <param name="sender">事件的触发对象</param>
		/// <param name="e">事件的附加参数</param>
		void ReceiveDiscussMessage (object sender, CqDiscussMessageEventArgs e);

		/// <summary>
		/// Type=21 讨论组私聊消息 <para/>
		/// 当在派生类中重写时, 处理收到的讨论组私聊消息
		/// </summary>
		/// <param name="sender">事件的触发对象</param>
		/// <param name="e">事件的附加参数</param>
		void ReceiveDiscussPrivateMessage (object sender, CqPrivateMessageEventArgs e);
	}
}
