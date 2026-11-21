[include:common_14-Si_parameter.inc]

[parameter]
  # ICRP Publ.151 p.67 Table 7.2 / Ingested materials, Silicon dixoide and silicates

$if parent
  ${Si}/fA    = 0.01
  ${Si}/fA_Re = fA_MaxValueOfIng    # footnote§
$else
  ${*}/fA     = fA_MaxValueOfIng
  ${*}/fA_Re  = fA_MaxValueOfIng
$endif


[include:../share/common_intake_ing.inc]

[include:../share/common_HATM+UB.inc]

[include:element_15-P.inc]
[include:element_14-Si.inc]
