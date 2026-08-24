[include:common_26-Fe_parameter.inc]

[parameter]
$if parent
  ${Fe}/fA    = fA_MaxValueOfIng
  ${Fe}/fA_Re = fA_MaxValueOfIng    # footnote¶
$else
  ${*}/fA     = fA_MaxValueOfIng
  ${*}/fA_Re  = fA_MaxValueOfIng
$endif

[include:../share/common_intake_inj.inc]

[include:../share/common_HATM+UB.inc]

[include:element_27-Co.inc]
[include:element_26-Fe.inc]
[include:element_25-Mn.inc]
