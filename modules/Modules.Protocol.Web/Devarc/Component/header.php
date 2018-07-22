<?php namespace Devarc\Component;

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
	
	function __construct($_request)
	{
		//$data = decrypt($_request);
		$args = json_decode($_request);
		$this->user_seq = $args->{'user_seq'};
		$this->user_key = $args->{'user_key'};
		$this->rmi_id = $args->{'rmi_id'};
		$this->rmi_data = $args->{'rmi_data'};
	}
}


$loader = new \Devarc\Component\Psr4AutoloaderClass('/var/www/html');
$loader->addNamespace('Devarc', '/Devarc');

?>
