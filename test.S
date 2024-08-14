.global _main

_main:

movl $29, 16(%esp)
movl $28, 24(%esp)
movl $30, 32(%esp)
movl 16(%esp), %eax
cmpl 24(%esp), %eax
jg L1
movl 24(%esp), %eax
cmpl 32(%esp), %eax
jg L2
movl $10, %eax
ret 
L1: 
movl $15, %eax
ret
L2: 
movl $17, %eax
ret
 