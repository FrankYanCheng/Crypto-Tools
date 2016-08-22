'''
This is the crypto tool for linux,use python-rsa.
Version 1.0
python 2.7
create time 2016-8-21 21:24:14
'''
#coding:utf-8
import rsa
import pickle
import os
import sys
from rsa.bigfile import *

class crypto(object):
	
	'''
	Read keys from pickle file
	'''
	def readPickle(self,isPublic_key):
		'''
		Read key information
		'''
		if isPublic_key==True:
			f=open(self.pickle_pub_file,'rb')
			self.pub_key=pickle.load(f)['pub_key']
			f.close()
		else:
			f=open(self.pickle_pri_file,'rb')
			self.pri_key=pickle.load(f)['pri_key']
			f.close()

	'''
	Write keys to pickle file
	'''
	def writePickle(self,isPublic_key):
		di_info={}
		if isPublic_key==True:
			f=open(self.pickle_pub_file,'wb')
			di_info['pub_key']=self.pub_key
			pickle.dump(di_info,f)
			f.close()
		else:
			f=open(self.pickle_pri_file,'wb')
			di_info['pri_key']=self.pri_key
			pickle.dump(di_info,f)
			f.close()
	
	def __init__(self,pickle_pub_file,pickle_pri_file):
		self.pickle_pub_file=pickle_pub_file
		self.pickle_pri_file=pickle_pri_file
		self.pub_key=None
		self.pri_key=None
	
	'''
	create the picke file or take the value of pub_key and pri_key
	'''
	def loadFile(self):
		if os.path.exists(self.pickle_pub_file)==False:
			self.writePickle(True)
		else:
			self.readPickle(True)
		if os.path.exists(self.pickle_pri_file)==False:
			self.writePickle(False)
		else:
			self.readPickle(False)
	'''
	read keys from file,do not create keys,it's used for the condition of exists keys
	'''
	def readFile(self):
		if os.path.exists(self.pickle_pub_file)==True:
			self.readPickle(True)
		if os.path.exists(self.pickle_pri_file)==True:
			self.readPickle(False)
	
	'''
	generate private key and public key
	'''
	def generateKey(self):
		(self.pub_key,self.pri_key)=rsa.newkeys(512)
		self.writePickle(True)
		self.writePickle(False)

	def encrypt(self,filename,encrypt_filename):
		with open(filename,'rb') as infile,open(encrypt_filename,'wb') as outfile:
			rsa.bigfile.encrypt_bigfile(infile,outfile,self.pub_key)
		
	def decrypt(self,encrypt_filename,filename):
		with open(encrypt_filename,'rb') as infile,open(filename,'wb') as outfile:
			rsa.bigfile.decrypt_bigfile(infile,outfile,self.pri_key)


class fileHelper(object):
	
	'''
	rename file,if success,return true else return false
	'''
	
	def rename(self,ori_name,after_name):
		'''
		if the file exist,rename file and return true
		'''
		if os.path.exists(after_name)==False:
			if os.path.exists(ori_name)==True:
				os.rename(ori_name,after_name)
				return True
		return False

	'''
	search a directory,return files
	'''

	def searchFile(self):
		files=set()
		for f in os.listdir(os.getcwd()):
			if os.path.isfile(f):
				files.add(f)
		return files

	'''
	encrypt the name,add 'temp' suffix
	'''	

	def encryptName(self,ori_name):	
		file_name=[]
		f_info_arr=os.path.splitext(ori_name)
		file_name.append(f_info_arr[0])
		file_name.append(f_info_arr[1])
		file_name[0]=file_name[0]+'temp'
		return file_name[0]+file_name[1]

	'''
	decrypt the name,delete 'temp' suffix
	'''
	
	def decryptName(self,encrypt_name):
		file_name=[]
		f_info_arr=os.path.splitext(encrypt_name)
		file_name.append(f_info_arr[0])
		file_name.append(f_info_arr[1])
		name=file_name[0]
		name=name[0:len(name)-4]
		return name+file_name[1]
			
pub_file='pub.obj'
pri_file='pri.obj'
print 'It has produced keys or load keys,please do not alter the key\'s name'
#crypto class
cypt=crypto(pub_file,pri_file)
ex_pub_file=os.path.exists(pub_file)
ex_pri_file=os.path.exists(pri_file)
if ex_pub_file==False and ex_pri_file==False:
	cypt.generateKey()
if ex_pub_file==True or ex_pri_file==True:
	cypt.readFile()
#file class
fh=fileHelper()
files=fh.searchFile()
this_file=os.path.realpath(__file__)
this_file=os.path.basename(this_file)
#remove some files that we don't want to encrypt or decrypt
remove_files={this_file,pub_file,pri_file}
files=files-remove_files

str_style=raw_input('Encrypt or Decrypt\n')
str_confirm=raw_input('Y/N\n')

if str.lower(str_confirm)=='y':

	if str.lower(str_style)=='encrypt':
		print 'Encrypting'
		for f in files:
			print '%s is encrypting' % f
			encrypt_filename=fh.encryptName(f)
			cypt.encrypt(f,encrypt_filename)
			os.remove(f)
	if str.lower(str_style)=='decrypt':
		for f in files:
			print '%s is decrypting' % f
			filename=fh.decryptName(f)
			cypt.decrypt(f,filename)
			os.remove(f)	
print 'Work has been down'
