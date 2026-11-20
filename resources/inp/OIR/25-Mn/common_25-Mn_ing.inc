[include:common_25-Mn_parameter.inc]

[parameter]
  # ICRP Publ.151 p.119 Table 14.2 / Ingested materials, All forms

$if parent
  ${Mn}/fA    = 0.05
  ${Mn}/fA_Re = fA_MaxValueOfIng    # footnote§
$else
  ${*}/fA     = fA_MaxValueOfIng
  ${*}/fA_Re  = fA_MaxValueOfIng
$endif


[include:../share/common_intake_ing.inc]

[include:../share/common_HATM+UB.inc]

[include:element_25-Mn.inc]
[include:element_24-Cr.inc]
