[include:common_81-Tl_parameter.inc]

[parameter]
  # ICRP Publ.151 p.344 Table 37.2 / Ingested materials, All forms

$if parent
  ${Tl}/fA    = 0.99 # ≒1
  ${Tl}/fA_Re = fA_MaxValueOfIng    # footnote§
$else
  ${*}/fA     = fA_MaxValueOfIng
  ${*}/fA_Re  = fA_MaxValueOfIng
$endif


[include:../share/common_intake_ing.inc]

[include:../share/common_HATM+UB.inc]

[include:element_81-Tl.inc]
[include:element_80-Hg.inc]
[include:element_79-Au.inc]
