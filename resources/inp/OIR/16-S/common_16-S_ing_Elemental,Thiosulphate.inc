[include:common_16-S_parameter_inorganic.inc]

[parameter]
  # ICRP Publ.134 p.104 Table 5.3 / Ingested materials, Elemental sulphur and thiosulphate

$if parent
  ${S}/fA    = 0.1
  ${S}/fA_Re = fA_MaxValueOfIng     # footnote††
$else
  ${*}/fA    = fA_MaxValueOfIng
  ${*}/fA_Re = fA_MaxValueOfIng
$endif


[include:common_intake_ing.inc]

[include:common_HATM+UB.inc]

[include:element_16-S_inorganic.inc]
[include:element_17-Cl_inorganic.inc]
