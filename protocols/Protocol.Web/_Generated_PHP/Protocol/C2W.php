<?php namespace Devarc\Protocol;
class C2W
{
	var $message = null;
	function __construct($_header, $_body)
	{
		$this->message = new \Devarc\Component\RMIMessage($_header, $_body);
	}
	public function dispatch()
	{
		switch($this->message->rmi_id)
		{
			case 30010: // Request_Login
				$proc = new \Devarc\Protocol\Request_Login($this->message);
				$proc->dispatch();
				break;
			case 30020: // Request_Stage_Clear
				$proc = new \Devarc\Protocol\Request_Stage_Clear($this->message);
				$proc->dispatch();
				break;
			default:
				echo '{"Error":1 ,"Message":"Not Implemented."}'; 
				break;
		}
	}
}
?>
