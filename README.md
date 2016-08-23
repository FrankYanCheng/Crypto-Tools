Crypto Tools for ‘linux‘ and ‘windows‘,you can use it to encrypt files or decrypt files,**windows version is not compatible with linux version**,use sra.if you want to see the theory

[阮一峰RSA原理解析](http://www.ruanyifeng.com/blog/2013/06/rsa_algorithm_part_one.html)
[How to use python rsa](https://stuvel.eu/python-rsa-doc/)
***
It will generate public key and private key if there are not exist  
input order encrypt or decrypt to crypto files,it will change those files in the directory where the program in.Save keys carefully,the process is irreversible.

**Windows Version**  
	This version use C#,it will crypto file and it's name.It will crypto small files (<512kb) entirly(you can change it) or crypto it partly.It not fit big files contain text because it will crypto it partly
	
**Linux Version**  
You can use order pip install rsa to install rsa module.  
This version use Python,it will only crypto files.After crypto,the file name will change to name+'temp'+'.xxx',please do not change it.It will crypto files entirly.It not fit big files,it will crypto slowly.

***
**This just a tool for fun,Please backup those files if it important.**
