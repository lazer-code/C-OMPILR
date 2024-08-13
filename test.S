.global _main

_main:

movl $25, 16(%esp)
movl $28, 24(%esp)
movl 16(%esp), %eax
cmpl 24(%esp), %eax
jg L1
movl $10, %eax
ret 
L1: 
movl $15, %eax
ret 
