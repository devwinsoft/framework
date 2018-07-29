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
		var_dump($this->message);
	}
}
?>