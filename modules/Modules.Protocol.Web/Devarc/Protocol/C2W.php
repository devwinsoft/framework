<?php namespace Devarc\Protocol;
class C2W
{
	var $message = null;
	function __construct($_request)
	{
		$this->message = new \Devarc\Component\RMIMessage($_request);
	}
	public function dispatch()
	{
		switch($this->message->rmi_id)
		{
			case 30010:
				$proc = new \Devarc\Protocol\Request_Login($this->message);
				$proc->dispatch();
				break;
			default:
				echo '{"ErrorMessage":"Not Implemented."}'; 
				break;
		}
	}
}
?>
