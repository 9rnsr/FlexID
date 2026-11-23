[include:common_48-Cd_parameter.inc]

[parameter]
  # ICRP Publ.151 p.238 Table 26.2 / Ingested materials, All compounds

$if parent
  ${Cd}/fA    = 0.05
  ${Cd}/fA_Re = fA_MaxValueOfIng    # footnote¶
$else
  ${*}/fA     = fA_MaxValueOfIng
  ${*}/fA_Re  = fA_MaxValueOfIng
$endif


[include:../share/common_intake_ing.inc]

[include:../share/common_HATM+UB.inc]

[include:element_50-Sn.inc]
[include:element_49-In.inc]
[include:element_48-Cd.inc]
[include:element_47-Ag.inc]
