[include:common_29-Cu_parameter.inc]

[parameter]
  # ICRP Publ.151 p.153 Table 16.2 / Ingested materials, All forms

$if parent
  ${Cu}/fA    = 0.5
  ${Cu}/fA_Re = fA_MaxValueOfIng    # footnote§
$else
  ${*}/fA     = fA_MaxValueOfIng
  ${*}/fA_Re  = fA_MaxValueOfIng
$endif


[include:../share/common_intake_ing.inc]

[include:../share/common_HATM+UB.inc]

[include:element_29-Cu.inc]
