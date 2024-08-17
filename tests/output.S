.global _main

_main:

movl $4, 16(%esp)
movl $15, 24(%esp)
movl $48, 32(%esp)
movl 16(%esp), %eax
cmpl 24(%esp), %eax
jg L1
movl 24(%esp), %eax
ret 
L1: 
movl 16(%esp), %eax
ret 
