<?php namespace Devarc\Protocol;
class C2W
{
	var $message = null;
	function __construct($_data, $_enc)
	{
		$this->message = new \Devarc\Component\RMIMessage($_data, $_enc);
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
				echo '{"ErrorMessage":"Not Implemented."}'; 
				break;
		}
	}
}
?>
