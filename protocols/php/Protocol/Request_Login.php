<?php namespace Devarc\Protocol;
class Request_Login
{
	var $message = null;
	
	function __construct($_message)
	{
		$this->message = $_message;
	}
	
	public function dispatch()
	{
		echo '{"Error":0, "Message":"' . \Devarc\Component\encrypt($this->message->rmi_data) . '"}'; 
	}
}
?>