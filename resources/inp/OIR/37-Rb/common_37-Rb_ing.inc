[include:common_37-Rb_parameter.inc]

[parameter]
  # ICRP Publ.151 p.202 Table 22.2 / Ingested materials, All forms

$if parent
  ${Rb}/fA    = 0.99 # ≒1
  ${Rb}/fA_Re = fA_MaxValueOfIng    # footnote§
$else
  ${*}/fA     = fA_MaxValueOfIng
  ${*}/fA_Re  = fA_MaxValueOfIng
$endif


[include:../share/common_intake_ing.inc]

[include:../share/common_HATM+UB.inc]

[include:element_38-Sr.inc]
[include:element_37-Rb.inc]
[include:element_36-Kr.inc]
