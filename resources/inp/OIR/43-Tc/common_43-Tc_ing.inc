[include:common_43-Tc_parameter.inc]

[parameter]
# ICRP Publ.134 p.322 Table 15.2 / Ingested materials, All forms

$if parent
  ${Tc}/fA    = 0.9
  ${Tc}/fA_Re = fA_MaxValueOfIng    # footnote¶
$else
  ${*}/fA     = fA_MaxValueOfIng
  ${*}/fA_Re  = fA_MaxValueOfIng
$endif

[include:../share/common_intake_ing.inc]

[include:../share/common_HATM+UB.inc]

[include:element_43-Tc.inc]
