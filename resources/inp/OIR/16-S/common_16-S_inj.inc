[include:common_16-S_parameter_inorganic.inc]

[parameter]
$if parent
  ${S}/fA    = fA_MaxValueOfIng
  ${S}/fA_Re = fA_MaxValueOfIng     # footnote††
$else
  ${*}/fA    = fA_MaxValueOfIng
  ${*}/fA_Re = fA_MaxValueOfIng
$endif


[include:common_intake_inj.inc]

[include:common_HATM+UB.inc]

[include:element_16-S_inorganic.inc]
[include:element_17-Cl_inorganic.inc]
