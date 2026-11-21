[include:common_24-Cr_parameter.inc]

[parameter]
  # ICRP Publ.151 p.109 Table 13.2 / Ingested materials, All forms

$if parent
  ${Cr}/fA    = 0.01
  ${Cr}/fA_Re = fA_MaxValueOfIng    # footnote§
$else
  ${*}/fA     = fA_MaxValueOfIng
  ${*}/fA_Re  = fA_MaxValueOfIng
$endif


[include:../share/common_intake_ing.inc]

[include:../share/common_HATM+UB.inc]

[include:element_24-Cr.inc]
[include:element_23-V.inc]
