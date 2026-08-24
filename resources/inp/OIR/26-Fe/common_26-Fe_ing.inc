[include:common_26-Fe_parameter.inc]

[parameter]
# ICRP Publ.134 p.136 Table 7.2 / Ingested materials, All unspecified forms

$if parent
  ${Fe}/fA    = 0.1
  ${Fe}/fA_Re = fA_MaxValueOfIng    # footnote¶
$else
  ${*}/fA     = fA_MaxValueOfIng
  ${*}/fA_Re  = fA_MaxValueOfIng
$endif

[include:../share/common_intake_ing.inc]

[include:../share/common_HATM+UB.inc]

[include:element_27-Co.inc]
[include:element_26-Fe.inc]
[include:element_25-Mn.inc]
