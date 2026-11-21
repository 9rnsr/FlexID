[include:common_11-Na_parameter.inc]

[parameter]
  # ICRP Publ.151 p.35 Table 4.2 / Ingested materials, All forms

$if parent
  ${Na}/fA    = 0.99 # ≒1
  ${Na}/fA_Re = fA_MaxValueOfIng    # footnote§
$else
  ${*}/fA     = fA_MaxValueOfIng
  ${*}/fA_Re  = fA_MaxValueOfIng
$endif


[include:../share/common_intake_ing.inc]

[include:../share/common_HATM+UB.inc]

[include:element_11-Na.inc]
