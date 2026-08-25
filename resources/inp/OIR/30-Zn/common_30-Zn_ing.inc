[include:common_30-Zn_parameter.inc]

[parameter]
# ICRP Publ.134 p.186 Table 9.2 / Ingested materials, All forms

$if parent
  ${Zn}/fA    = 0.5
  ${Zn}/fA_Re = fA_MaxValueOfIng    # footnote¶
$else
  ${^Cu}/fA    = fA_MaxValueOfIng
  ${^Cu}/fA_Re = fA_MaxValueOfIng
$endif

[include:common_intake_ing.inc]

[include:common_HATM+UB.inc]

[include:element_31-Ga.inc]
[include:element_30-Zn.inc]
[include:element_29-Cu.inc]
