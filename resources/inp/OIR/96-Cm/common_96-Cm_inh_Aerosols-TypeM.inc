[include:common_96-Cm_parameter.inc]

[parameter]
  # ICRP Publ.141 p.408 Table 24.7 / Inhaled particulate materials, Absorption type M

  $fr = 0.2
  $sr = 0.4
  $ss = 0.005
  $fb = 0.002       # footnote*
  $sb = 0           # footnote*

$if parent
  ${Cm}/fA     = 1E-4
  ${Cm}/fA_Re  = fA_MaxValueOfIng   # footnote**
$else
  ${^Rn}/fA    = fr * fA_MaxValueOfIng
  ${^Rn}/fA_Re =      fA_MaxValueOfIng
$endif

[include:common_intake_inh.inc]

[include:common_HRTM.inc]
[include:common_HATM+UB.inc]

[include:element_98-Cf.inc]
[include:element_97-Bk.inc]
[include:element_96-Cm.inc]
[include:element_95-Am.inc]
[include:element_94-Pu.inc]
[include:element_93-Np.inc]
[include:element_92-U.inc]
[include:element_91-Pa.inc]
[include:element_90-Th.inc]
[include:element_89-Ac.inc]
[include:element_88-Ra.inc]
[include:element_87-Fr.inc]
[include:element_86-Rn_inh.inc]
[include:element_85-At.inc]
[include:element_84-Po.inc]
[include:element_83-Bi.inc]
[include:element_82-Pb.inc]
[include:element_81-Tl.inc]
