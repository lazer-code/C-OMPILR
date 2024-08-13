_main:

movl 16(%esp), $25
movl 24(%esp), $28
cmp 24(%esp), 16(%esp)
jg L1
mov eax, $10
ret 
L1: 
mov eax, $15
ret 
