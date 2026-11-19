[include:common_78-Pt_parameter.inc]

[parameter]
  # ICRP Publ.151 p.305 Table 34.2 / Ingested materials, Metal, oxide and hydroxide

$if parent
  ${Pt}/fA    = 0.001
  ${Pt}/fA_Re = fA_MaxValueOfIng    # footnote§
$else
  ${*}/fA     = fA_MaxValueOfIng
  ${*}/fA_Re  = fA_MaxValueOfIng
$endif

[include:../share/common_intake_ing.inc]

[include:../share/common_HATM+UB.inc]

[include:element_79-Au.inc]
[include:element_78-Pt.inc]
[include:element_77-Ir.inc]
[include:element_76-Os.inc]
[include:element_75-Re.inc]
