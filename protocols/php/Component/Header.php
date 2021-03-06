<?php namespace Devarc\Component;

global $conf;
$conf["encrypt_bit"] = 8;
$conf["encrypt_iv"] = "awwxewwo";
$conf["encrypt_key"] = "xiinwwwWwW@aw()r@@@rrrss";

function encrypt($text)
{
	global $conf;			
	$text_num =str_split($text,$conf["encrypt_bit"]);
	$text_num = $conf["encrypt_bit"]-strlen($text_num[count($text_num)-1]);
	for ($i=0;$i<$text_num; $i++)
	{
		$text = $text . chr($text_num);
	}
	$cipher = mcrypt_module_open(MCRYPT_TRIPLEDES,'','cbc','');
	mcrypt_generic_init($cipher, $conf["encrypt_key"], $conf["encrypt_iv"]);
	$decrypted = mcrypt_generic($cipher,$text);
	mcrypt_generic_deinit($cipher);
	return base64_encode($decrypted);
}

function decrypt($encrypted_text)
{
	global $conf;
	$cipher = mcrypt_module_open(MCRYPT_TRIPLEDES,'','cbc','');
	mcrypt_generic_init($cipher, $conf["encrypt_key"], $conf["encrypt_iv"]);
	$decrypted = mdecrypt_generic($cipher,base64_decode($encrypted_text));
	mcrypt_generic_deinit($cipher);
	$last_char=substr($decrypted,-1);
	for($i=0;$i<$conf["encrypt_bit"]-1; $i++)
	{
		if(chr($i)==$last_char)
		{
			$decrypted=substr($decrypted,0,strlen($decrypted)-$i);
			break;
		}
	}
	return $decrypted;
}

class Psr4AutoloaderClass
{
    protected $prefixes = array();
	protected $rootDir;

	function __construct($_rootDir)
	{
		$this->rootDir = $_rootDir;
		$this->register();
    }
	
    public function register()
    {
        spl_autoload_register(array($this, 'loadClass'));
    }

    public function addNamespace($prefix, $base_dir, $prepend = false)
    {
        // normalize namespace prefix
        $prefix = trim($prefix, '\\') . '\\';

        // normalize the base directory with a trailing separator
        $base_dir = rtrim($this->rootDir . $base_dir, DIRECTORY_SEPARATOR) . '/';

        // initialize the namespace prefix array
        if (isset($this->prefixes[$prefix]) === false) {
            $this->prefixes[$prefix] = array();
        }

        // retain the base directory for the namespace prefix
        if ($prepend) {
            array_unshift($this->prefixes[$prefix], $base_dir);
        } else {
            array_push($this->prefixes[$prefix], $base_dir);
        }
    }

    public function loadClass($class)
    {
        // the current namespace prefix
        $prefix = $class;

        while (false !== $pos = strrpos($prefix, '\\')) {

            // retain the trailing namespace separator in the prefix
            $prefix = substr($class, 0, $pos + 1);

            // the rest is the relative class name
            $relative_class = substr($class, $pos + 1);

            // try to load a mapped file for the prefix and relative class
            $mapped_file = $this->loadMappedFile($prefix, $relative_class);
            if ($mapped_file) {
                return $mapped_file;
            }

            $prefix = rtrim($prefix, '\\');
        }
        return false;
    }

    protected function loadMappedFile($prefix, $relative_class)
    {
        // are there any base directories for this namespace prefix?
        if (isset($this->prefixes[$prefix]) === false) {
            return false;
        }

        foreach ($this->prefixes[$prefix] as $base_dir) {

            $file = $base_dir
                  . str_replace('\\', '/', $relative_class)
                  . '.php';

            // if the mapped file exists, require it
            if ($this->requireFile($file)) {
                // yes, we're done
                return $file;
            }
        }

        return false;
    }

    protected function requireFile($file)
    {
        if (file_exists($file)) {
            require $file;
            return true;
        }
        return false;
    }
}


class RMIMessage
{
	var $user_seq;
	var $user_key;
	var $rmi_id;
	var $rmi_data;
	
	function __construct($_header, $_body)
	{
		$temp = \Devarc\Component\decrypt($_header);
		$args = json_decode($temp);
		$this->user_seq = $args->{'user_seq'};
		$this->user_key = $args->{'user_key'};
		$this->rmi_id = $args->{'rmi_id'};
		$this->rmi_data = \Devarc\Component\decrypt($_body);
	}
}


$loader = new \Devarc\Component\Psr4AutoloaderClass('/var/www/html');
$loader->addNamespace('Devarc', '/Devarc');
//$loader->addNamespace('Devarc/Component', '/Devarc/Component');
//$loader->addNamespace('Devarc/Protocol', '/Devarc/Protocol');

?>
