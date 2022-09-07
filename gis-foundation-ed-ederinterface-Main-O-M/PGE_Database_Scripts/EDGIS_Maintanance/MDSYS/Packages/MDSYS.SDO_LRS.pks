Prompt drop Package SDO_LRS;
DROP PACKAGE MDSYS.SDO_LRS
/

Prompt Package SDO_LRS;
--
-- SDO_LRS  (Package) 
--
CREATE OR REPLACE PACKAGE MDSYS.sdo_lrs AUTHID current_user AS

--
--
-- functions/procedure for defining, creating and manipulating LRS geometric segments
--
--

   --
   -- define an LRS geometric segment, all measures will be automatically populated
   --

   PROCEDURE define_geom_segment(geom_segment  IN OUT MDSYS.SDO_GEOMETRY,
   			         dim_array     IN MDSYS.SDO_DIM_ARRAY,
   			         start_measure IN NUMBER ,
   			         end_measure   IN NUMBER );
   -- PRAGMA RESTRICT_REFERENCES(define_geom_segment, wnds, rnps, wnps);

   PROCEDURE define_geom_segment(geom_segment  IN OUT MDSYS.SDO_GEOMETRY,
   			         dim_array     IN MDSYS.SDO_DIM_ARRAY);

   -- PRAGMA RESTRICT_REFERENCES(define_geom_segment, wnds, rnps, wnps);


   PROCEDURE define_geom_segment(geom_segment  IN OUT MDSYS.SDO_GEOMETRY,
   			         start_measure IN NUMBER ,
   			         end_measure   IN NUMBER ,
                                 tolerance     IN NUMBER DEFAULT 1.0e-8);
   -- PRAGMA RESTRICT_REFERENCES(define_geom_segment, wnds, rnps, wnps);


   PROCEDURE define_geom_segment(geom_segment  IN OUT MDSYS.SDO_GEOMETRY,
                                 tolerance     IN NUMBER DEFAULT 1.0e-8);
   -- PRAGMA RESTRICT_REFERENCES(define_geom_segment, wnds, rnps, wnps);



   PROCEDURE define_geom_segment_3d(geom_segment  IN OUT MDSYS.SDO_GEOMETRY,
   			            dim_array     IN MDSYS.SDO_DIM_ARRAY,
   			            start_measure IN NUMBER ,
   			            end_measure   IN NUMBER );
   -- PRAGMA RESTRICT_REFERENCES(define_geom_segment_3d, wnds, rnps, wnps);

   PROCEDURE define_geom_segment_3d(geom_segment  IN OUT MDSYS.SDO_GEOMETRY,
   			            dim_array     IN MDSYS.SDO_DIM_ARRAY);

   -- PRAGMA RESTRICT_REFERENCES(define_geom_segment_3d, wnds, rnps, wnps);

   PROCEDURE define_geom_segment_3d(geom_segment  IN OUT MDSYS.SDO_GEOMETRY,
   			            start_measure IN NUMBER ,
   			            end_measure   IN NUMBER ,
                                    tolerance     IN NUMBER DEFAULT 1.0e-8);
   -- PRAGMA RESTRICT_REFERENCES(define_geom_segment_3d, wnds, rnps, wnps);

   PROCEDURE define_geom_segment_3d(geom_segment  IN OUT MDSYS.SDO_GEOMETRY,
                                    tolerance     IN NUMBER DEFAULT 1.0e-8);
   -- PRAGMA RESTRICT_REFERENCES(define_geom_segment_3d, wnds, rnps, wnps);






   --
   -- create a new geometric segment by clipping the given geometric segment
   --

   FUNCTION clip_geom_segment(geom_segment  IN MDSYS.SDO_GEOMETRY,
			      dim_array     IN MDSYS.SDO_DIM_ARRAY,
			      start_measure IN NUMBER,
			      end_measure   IN NUMBER)
   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(clip_geom_segment, wnds, rnps, wnps);

   FUNCTION clip_geom_segment(geom_segment  IN MDSYS.SDO_GEOMETRY,
			      start_measure IN NUMBER,
			      end_measure   IN NUMBER,
			      tolerance     IN NUMBER DEFAULT 1.0e-8)
   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(clip_geom_segment, wnds, rnps, wnps);



   FUNCTION clip_geom_segment_3d(geom_segment  IN MDSYS.SDO_GEOMETRY,
			         dim_array     IN MDSYS.SDO_DIM_ARRAY,
			         start_measure IN NUMBER,
			         end_measure   IN NUMBER)
   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(clip_geom_segment_3d, wnds, rnps, wnps);

   FUNCTION clip_geom_segment_3d(geom_segment  IN MDSYS.SDO_GEOMETRY,
			         start_measure IN NUMBER,
			         end_measure   IN NUMBER,
				 tolerance     IN NUMBER DEFAULT 1.0e-8)
   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(clip_geom_segment_3d, wnds, rnps, wnps);


   --
   -- split a geometric segment into two
   --


   PROCEDURE split_geom_segment(geom_segment  IN  MDSYS.SDO_GEOMETRY,
			        dim_array     IN  MDSYS.SDO_DIM_ARRAY,
			        split_measure IN  NUMBER,
			        segment_1     OUT MDSYS.SDO_GEOMETRY,
			        segment_2     OUT MDSYS.SDO_GEOMETRY);
   PRAGMA RESTRICT_REFERENCES(split_geom_segment, wnds, rnps, wnps);

   PROCEDURE split_geom_segment(geom_segment  IN  MDSYS.SDO_GEOMETRY,
			        split_measure IN  NUMBER,
			        segment_1     OUT MDSYS.SDO_GEOMETRY,
			        segment_2     OUT MDSYS.SDO_GEOMETRY,
                                tolerance     IN  NUMBER DEFAULT 1.0e-8);
   PRAGMA RESTRICT_REFERENCES(split_geom_segment, wnds, rnps, wnps);


   PROCEDURE split_geom_segment_3d(geom_segment  IN  MDSYS.SDO_GEOMETRY,
			           dim_array     IN  MDSYS.SDO_DIM_ARRAY,
			           split_measure IN  NUMBER,
			           segment_1     OUT MDSYS.SDO_GEOMETRY,
			           segment_2     OUT MDSYS.SDO_GEOMETRY);
   PRAGMA RESTRICT_REFERENCES(split_geom_segment_3d, wnds, rnps, wnps);

   PROCEDURE split_geom_segment_3d(geom_segment  IN  MDSYS.SDO_GEOMETRY,
			           split_measure IN  NUMBER,
			           segment_1     OUT MDSYS.SDO_GEOMETRY,
			           segment_2     OUT MDSYS.SDO_GEOMETRY,
                                   tolerance     IN  NUMBER DEFAULT 1.0e-8);
   PRAGMA RESTRICT_REFERENCES(split_geom_segment_3d, wnds, rnps, wnps);


   --
   -- concatenate two geometric segments into one
   --

   FUNCTION concatenate_geom_segments(geom_segment_1 IN MDSYS.SDO_GEOMETRY,
  				      dim_array_1    IN MDSYS.SDO_DIM_ARRAY,
  				      geom_segment_2 IN MDSYS.SDO_GEOMETRY,
  				      dim_array_2    IN MDSYS.SDO_DIM_ARRAY)

   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(concatenate_geom_segments, wnds, rnps, wnps);

   FUNCTION concatenate_geom_segments(geom_segment_1 IN MDSYS.SDO_GEOMETRY,
  				      geom_segment_2 IN MDSYS.SDO_GEOMETRY,
				      tolerance      IN NUMBER DEFAULT 1.0e-8)

   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(concatenate_geom_segments, wnds, rnps, wnps);



   FUNCTION concatenate_geom_segments_3d(geom_segment_1 IN MDSYS.SDO_GEOMETRY,
  				         dim_array_1    IN MDSYS.SDO_DIM_ARRAY,
  				         geom_segment_2 IN MDSYS.SDO_GEOMETRY,
  				         dim_array_2    IN MDSYS.SDO_DIM_ARRAY)

   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(concatenate_geom_segments_3d, wnds, rnps, wnps);

   FUNCTION concatenate_geom_segments_3d(geom_segment_1 IN MDSYS.SDO_GEOMETRY,
  				         geom_segment_2 IN MDSYS.SDO_GEOMETRY,
				         tolerance      IN NUMBER DEFAULT 1.0e-8)

   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(concatenate_geom_segments_3d, wnds, rnps, wnps);




   --
   -- general segment scaling function
   --

   FUNCTION scale_geom_segment(geom_segment  IN MDSYS.SDO_GEOMETRY,
			       dim_array     IN MDSYS.SDO_DIM_ARRAY,
			       start_measure IN NUMBER,
			       end_measure   IN NUMBER,
			       shift_measure IN NUMBER)
   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   -- PRAGMA RESTRICT_REFERENCES(scale_geom_segment, wnds, rnps, wnps);

   FUNCTION scale_geom_segment(geom_segment  IN MDSYS.SDO_GEOMETRY,
			       start_measure IN NUMBER,
			       end_measure   IN NUMBER,
			       shift_measure IN NUMBER,
                               tolerance     IN NUMBER DEFAULT 1.0e-8)
   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   -- PRAGMA RESTRICT_REFERENCES(scale_geom_segment, wnds, rnps, wnps);



--
--
-- LRS query functions/procedures
--
--

   --
   -- check if the given geometric segment is valid
   --

   FUNCTION valid_geom_segment(geom_segment IN MDSYS.SDO_GEOMETRY,
			       dim_array    IN MDSYS.SDO_DIM_ARRAY)
   RETURN VARCHAR2 PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(valid_geom_segment, wnds, rnps, wnps);

   FUNCTION valid_geom_segment(geom_segment IN MDSYS.SDO_GEOMETRY)
   RETURN VARCHAR2 PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(valid_geom_segment, wnds, rnps, wnps);


   FUNCTION valid_geom_segment_3d(geom_segment IN MDSYS.SDO_GEOMETRY,
			          dim_array    IN MDSYS.SDO_DIM_ARRAY)
   RETURN VARCHAR2 PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(valid_geom_segment_3d, wnds, rnps, wnps);

   FUNCTION valid_geom_segment_3d(geom_segment IN MDSYS.SDO_GEOMETRY)
   RETURN VARCHAR2 PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(valid_geom_segment_3d, wnds, rnps, wnps);



   --
   -- check if the measure information of a given geometric segment is defined
   -- the geom segment must be valid, (type, length, and no of dim, etc)
   --

   FUNCTION is_geom_segment_defined(geom_segment IN MDSYS.SDO_GEOMETRY,
			            dim_array    IN MDSYS.SDO_DIM_ARRAY)
   RETURN VARCHAR2 PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(is_geom_segment_defined, wnds, rnps, wnps);

   FUNCTION is_geom_segment_defined(geom_segment IN MDSYS.SDO_GEOMETRY)
   RETURN VARCHAR2 PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(is_geom_segment_defined, wnds, rnps, wnps);


   FUNCTION is_geom_segment_defined_3d(geom_segment IN MDSYS.SDO_GEOMETRY,
			               dim_array    IN MDSYS.SDO_DIM_ARRAY)
   RETURN VARCHAR2 PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(is_geom_segment_defined_3d, wnds, rnps, wnps);

   FUNCTION is_geom_segment_defined_3d(geom_segment IN MDSYS.SDO_GEOMETRY)
   RETURN VARCHAR2 PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(is_geom_segment_defined_3d, wnds, rnps, wnps);



   --
   -- check if the given LRS point is valid
   --


   FUNCTION valid_lrs_pt(point      IN MDSYS.SDO_GEOMETRY,
			 dim_array  IN MDSYS.SDO_DIM_ARRAY)
   RETURN VARCHAR2 PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(valid_lrs_pt, wnds, rnps, wnps);

   FUNCTION valid_lrs_pt(point      IN MDSYS.SDO_GEOMETRY)
   RETURN VARCHAR2 PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(valid_lrs_pt, wnds, rnps, wnps);


   FUNCTION valid_lrs_pt_3d(point      IN MDSYS.SDO_GEOMETRY,
			    dim_array  IN MDSYS.SDO_DIM_ARRAY)
   RETURN VARCHAR2 PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(valid_lrs_pt_3d, wnds, rnps, wnps);

   FUNCTION valid_lrs_pt_3d(point      IN MDSYS.SDO_GEOMETRY)
   RETURN VARCHAR2 PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(valid_lrs_pt_3d, wnds, rnps, wnps);



   --
   -- check if the given measure falls within the start/end measures
   -- of a given geometric segment
   --


   FUNCTION valid_measure(geom_segment IN MDSYS.SDO_GEOMETRY,
			  dim_array    IN MDSYS.SDO_DIM_ARRAY,
			  measure      IN NUMBER)
   RETURN VARCHAR2 PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(valid_measure, wnds, rnps, wnps);

   FUNCTION valid_measure(geom_segment IN MDSYS.SDO_GEOMETRY,
			  measure      IN NUMBER)
   RETURN VARCHAR2 PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(valid_measure, wnds, rnps, wnps);


   FUNCTION valid_measure_3d(geom_segment IN MDSYS.SDO_GEOMETRY,
			     dim_array    IN MDSYS.SDO_DIM_ARRAY,
			     measure      IN NUMBER)
   RETURN VARCHAR2 PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(valid_measure_3d, wnds, rnps, wnps);

   FUNCTION valid_measure_3d(geom_segment IN MDSYS.SDO_GEOMETRY,
			     measure      IN NUMBER)
   RETURN VARCHAR2 PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(valid_measure_3d, wnds, rnps, wnps);





   --
   -- check if two given geometric segments are connected
   --

   FUNCTION connected_geom_segments(geom_segment_1 IN MDSYS.SDO_GEOMETRY,
  				    dim_array_1    IN MDSYS.SDO_DIM_ARRAY,
  				    geom_segment_2 IN MDSYS.SDO_GEOMETRY,
  				    dim_array_2    IN MDSYS.SDO_DIM_ARRAY)

   RETURN VARCHAR2 PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(connected_geom_segments, wnds, rnps, wnps);

   FUNCTION connected_geom_segments(geom_segment_1 IN MDSYS.SDO_GEOMETRY,
  				    geom_segment_2 IN MDSYS.SDO_GEOMETRY,
				    tolerance      IN NUMBER)


   RETURN VARCHAR2 PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(connected_geom_segments, wnds, rnps, wnps);


   FUNCTION connected_geom_segments_3d(geom_segment_1 IN MDSYS.SDO_GEOMETRY,
  				       dim_array_1    IN MDSYS.SDO_DIM_ARRAY,
  				       geom_segment_2 IN MDSYS.SDO_GEOMETRY,
  				       dim_array_2    IN MDSYS.SDO_DIM_ARRAY)

   RETURN VARCHAR2 PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(connected_geom_segments_3d, wnds, rnps, wnps);

   FUNCTION connected_geom_segments_3d(geom_segment_1 IN MDSYS.SDO_GEOMETRY,
  				       geom_segment_2 IN MDSYS.SDO_GEOMETRY,
				       tolerance      IN NUMBER)
   RETURN VARCHAR2 PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(connected_geom_segments_3d, wnds, rnps, wnps);




   --
   -- return the length of the given geometric segment
   --

   FUNCTION geom_segment_length(geom_segment IN MDSYS.SDO_GEOMETRY,
			        dim_array    IN MDSYS.SDO_DIM_ARRAY)
   RETURN NUMBER PARALLEL_ENABLE;
   -- PRAGMA restrict_references(geom_segment_length, wnds, rnps, wnps);

   FUNCTION geom_segment_length(geom_segment IN MDSYS.SDO_GEOMETRY,
			        tolerance    IN NUMBER DEFAULT 1.0e-8)

   RETURN NUMBER PARALLEL_ENABLE;
   -- PRAGMA restrict_references(geom_segment_length, wnds, rnps, wnps);


   FUNCTION geom_segment_length_3d(geom_segment IN MDSYS.SDO_GEOMETRY,
			           dim_array    IN MDSYS.SDO_DIM_ARRAY)

   RETURN NUMBER PARALLEL_ENABLE;
   -- PRAGMA restrict_references(geom_segment_length_3d, wnds, rnps, wnps);


   FUNCTION geom_segment_length_3d(geom_segment IN MDSYS.SDO_GEOMETRY,
				   tolerance    IN NUMBER DEFAULT 1.0e-8)

   RETURN NUMBER PARALLEL_ENABLE;
   -- PRAGMA restrict_references(geom_segment_length_3d, wnds, rnps, wnps);



   --
   -- return the start point of the geometric segment
   --

   FUNCTION geom_segment_start_pt(geom_segment IN MDSYS.SDO_GEOMETRY,
  			          dim_array    IN MDSYS.SDO_DIM_ARRAY)
   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(geom_segment_start_pt, wnds, rnps, wnps);

   FUNCTION geom_segment_start_pt(geom_segment IN MDSYS.SDO_GEOMETRY)
   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(geom_segment_start_pt, wnds, rnps, wnps);

   FUNCTION geom_segment_start_pt_3d(geom_segment IN MDSYS.SDO_GEOMETRY,
  			             dim_array    IN MDSYS.SDO_DIM_ARRAY)
   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(geom_segment_start_pt_3d, wnds, rnps, wnps);

   FUNCTION geom_segment_start_pt_3d(geom_segment IN MDSYS.SDO_GEOMETRY)
   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(geom_segment_start_pt_3d, wnds, rnps, wnps);


   --
   -- return the start measure of the geometric segment
   --


   FUNCTION geom_segment_start_measure(geom_segment IN MDSYS.SDO_GEOMETRY,
  			               dim_array    IN MDSYS.SDO_DIM_ARRAY)
   RETURN NUMBER PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(geom_segment_start_measure, wnds, rnps, wnps);

   FUNCTION geom_segment_start_measure(geom_segment IN MDSYS.SDO_GEOMETRY)
   RETURN NUMBER PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(geom_segment_start_measure, wnds, rnps, wnps);


   FUNCTION geom_segment_start_measure_3d(geom_segment IN MDSYS.SDO_GEOMETRY,
  			                  dim_array    IN MDSYS.SDO_DIM_ARRAY)
   RETURN NUMBER PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(geom_segment_start_measure_3d, wnds, rnps, wnps);

   FUNCTION geom_segment_start_measure_3d(geom_segment IN MDSYS.SDO_GEOMETRY)
   RETURN NUMBER PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(geom_segment_start_measure_3d, wnds, rnps, wnps);


   --
   -- return the end point of the geometric segment
   --

   FUNCTION geom_segment_end_pt(geom_segment IN MDSYS.SDO_GEOMETRY,
  			        dim_array    IN MDSYS.SDO_DIM_ARRAY)
   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(geom_segment_end_pt, wnds, rnps, wnps);

   FUNCTION geom_segment_end_pt(geom_segment IN MDSYS.SDO_GEOMETRY)
   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(geom_segment_end_pt, wnds, rnps, wnps);


   FUNCTION geom_segment_end_pt_3d(geom_segment IN MDSYS.SDO_GEOMETRY,
  			           dim_array    IN MDSYS.SDO_DIM_ARRAY)
   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(geom_segment_end_pt_3d, wnds, rnps, wnps);

   FUNCTION geom_segment_end_pt_3d(geom_segment IN MDSYS.SDO_GEOMETRY)
   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(geom_segment_end_pt_3d, wnds, rnps, wnps);


   --
   -- return the end point of the geometric segment
   --

   FUNCTION geom_segment_end_measure(geom_segment IN MDSYS.SDO_GEOMETRY,
  			             dim_array    IN MDSYS.SDO_DIM_ARRAY)
   RETURN NUMBER PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(geom_segment_end_measure, wnds, rnps, wnps);

   FUNCTION geom_segment_end_measure(geom_segment IN MDSYS.SDO_GEOMETRY)

   RETURN NUMBER PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(geom_segment_end_measure, wnds, rnps, wnps);


   FUNCTION geom_segment_end_measure_3d(geom_segment IN MDSYS.SDO_GEOMETRY,
  			                dim_array    IN MDSYS.SDO_DIM_ARRAY)
   RETURN NUMBER PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(geom_segment_end_measure_3d, wnds, rnps, wnps);

   FUNCTION geom_segment_end_measure_3d(geom_segment IN MDSYS.SDO_GEOMETRY)
   RETURN NUMBER PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(geom_segment_end_measure_3d, wnds, rnps, wnps);




   --
   -- return the measure of a LSR point
   --

   FUNCTION get_measure(point     IN MDSYS.SDO_GEOMETRY,
			dim_array IN MDSYS.SDO_DIM_ARRAY)
   RETURN NUMBER PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(get_measure, wnds, rnps, wnps);

   FUNCTION get_measure(point     IN MDSYS.SDO_GEOMETRY)
   RETURN NUMBER PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(get_measure, wnds, rnps, wnps);


   FUNCTION get_measure_3d(point     IN MDSYS.SDO_GEOMETRY,
			   dim_array IN MDSYS.SDO_DIM_ARRAY)
   RETURN NUMBER PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(get_measure_3d, wnds, rnps, wnps);


   FUNCTION get_measure_3d(point     IN MDSYS.SDO_GEOMETRY)
   RETURN NUMBER PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(get_measure_3d, wnds, rnps, wnps);


   --
   -- locate the point w.r.t a given geometric segment
   --

   FUNCTION locate_pt(geom_segment IN MDSYS.SDO_GEOMETRY,
		      dim_array    IN MDSYS.SDO_DIM_ARRAY,
		      measure      IN NUMBER,
		      offset       IN NUMBER DEFAULT 0)
   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(locate_pt, wnds, rnps, wnps);


   FUNCTION locate_pt(geom_segment IN MDSYS.SDO_GEOMETRY,
		      measure      IN NUMBER,
		      offset       IN NUMBER default 0,
                      tolerance    IN NUMBER default 1.0e-8)
   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(locate_pt, wnds, rnps, wnps);


   FUNCTION locate_pt_3d(geom_segment IN MDSYS.SDO_GEOMETRY,
		         dim_array    IN MDSYS.SDO_DIM_ARRAY,
		         measure      IN NUMBER)

   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(locate_pt_3d, wnds, rnps, wnps);

   FUNCTION locate_pt_3d(geom_segment IN MDSYS.SDO_GEOMETRY,
		         measure      IN NUMBER,
                         tolerance    IN NUMBER DEFAULT 1.0e-8)

   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(locate_pt_3d, wnds, rnps, wnps);





   --
   -- return the projection point of a given point w.r.t. to a given geometric segment
   --

   FUNCTION project_pt(geom_segment    IN MDSYS.SDO_GEOMETRY,
		       dim_array       IN MDSYS.SDO_DIM_ARRAY,
		       point           IN MDSYS.SDO_GEOMETRY,
		       point_dim_array IN MDSYS.SDO_DIM_ARRAY )

   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(project_pt, wnds, rnps, wnps);


   FUNCTION project_pt(geom_segment    IN MDSYS.SDO_GEOMETRY,
		       dim_array       IN MDSYS.SDO_DIM_ARRAY,
		       point           IN MDSYS.SDO_GEOMETRY)


   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(project_pt, wnds, rnps, wnps);


   FUNCTION project_pt(geom_segment    IN  MDSYS.SDO_GEOMETRY,
		       dim_array       IN  MDSYS.SDO_DIM_ARRAY,
		       point           IN  MDSYS.SDO_GEOMETRY,
		       point_dim_array IN  MDSYS.SDO_DIM_ARRAY,
                       offset          OUT NUMBER)

   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(project_pt, wnds, rnps, wnps);




   FUNCTION project_pt(geom_segment    IN MDSYS.SDO_GEOMETRY,
		       point           IN MDSYS.SDO_GEOMETRY,
                       tolerance       IN NUMBER DEFAULT 1.0e-8)


   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(project_pt, wnds, rnps, wnps);


   FUNCTION project_pt(geom_segment    IN  MDSYS.SDO_GEOMETRY,
		       point           IN  MDSYS.SDO_GEOMETRY,
                       tolerance       IN  NUMBER,
                       offset          OUT NUMBER )


   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(project_pt, wnds, rnps, wnps);



   FUNCTION project_pt_3d(geom_segment    IN MDSYS.SDO_GEOMETRY,
		          dim_array       IN MDSYS.SDO_DIM_ARRAY,
		          point           IN MDSYS.SDO_GEOMETRY,
		          point_dim_array IN MDSYS.SDO_DIM_ARRAY )

   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(project_pt_3d, wnds, rnps, wnps);

   FUNCTION project_pt_3d(geom_segment    IN MDSYS.SDO_GEOMETRY,
		          dim_array       IN MDSYS.SDO_DIM_ARRAY,
		          point           IN MDSYS.SDO_GEOMETRY)


   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(project_pt_3d, wnds, rnps, wnps);


   FUNCTION project_pt_3d(geom_segment    IN  MDSYS.SDO_GEOMETRY,
		          dim_array       IN  MDSYS.SDO_DIM_ARRAY,
		          point           IN  MDSYS.SDO_GEOMETRY,
		          point_dim_array IN  MDSYS.SDO_DIM_ARRAY,
                          offset          OUT NUMBER)

   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(project_pt_3d, wnds, rnps, wnps);



   FUNCTION project_pt_3d(geom_segment    IN MDSYS.SDO_GEOMETRY,
	          	  point           IN MDSYS.SDO_GEOMETRY,
                          tolerance       IN NUMBER DEFAULT 1.0e-8)
   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(project_pt_3d, wnds, rnps, wnps);


   FUNCTION project_pt_3d(geom_segment    IN  MDSYS.SDO_GEOMETRY,
		          point           IN  MDSYS.SDO_GEOMETRY,
                          tolerance       IN  NUMBER,
                          offset          OUT NUMBER)
   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(project_pt_3d, wnds, rnps, wnps);






---
--- LRS 817 features declaration
---



   --
   -- return geom segment measure range
   --

   FUNCTION measure_range(geom_segment IN MDSYS.SDO_GEOMETRY,
                          dim_array    IN MDSYS.SDO_DIM_ARRAY)
   RETURN NUMBER PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(measure_range, wnds, rnps, wnps);

   FUNCTION measure_range(geom_segment IN MDSYS.SDO_GEOMETRY)
   RETURN NUMBER PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(measure_range, wnds, rnps, wnps);



   FUNCTION measure_range_3d(geom_segment IN MDSYS.SDO_GEOMETRY,
                             dim_array    IN MDSYS.SDO_DIM_ARRAY)
   RETURN NUMBER PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(measure_range_3d, wnds, rnps, wnps);

   FUNCTION measure_range_3d(geom_segment IN MDSYS.SDO_GEOMETRY)
   RETURN NUMBER PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(measure_range_3d, wnds, rnps, wnps);





   --
   -- Convert standard dim info to lrs diminfo by adding an additional measure informaiotn
   --

   FUNCTION convert_to_lrs_dim_array(dim_array   IN MDSYS.SDO_DIM_ARRAY,
				     dim_name	 IN VARCHAR2,
	                             dim_pos	 IN INTEGER,
			             lower_bound IN NUMBER DEFAULT NULL,
                                     upper_bound IN NUMBER DEFAULT NULL,
	                             tolerance   IN NUMBER DEFAULT NULL)

   RETURN MDSYS.SDO_DIM_ARRAY PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(convert_to_lrs_dim_array, wnds, rnps, wnps);


   --
   -- Convert standard dim info to lrs diminfo by adding an additional measure informaiotn
   --

   FUNCTION convert_to_lrs_dim_array(dim_array   IN MDSYS.SDO_DIM_ARRAY,
				     dim_name	 IN VARCHAR2,
			             lower_bound IN NUMBER DEFAULT NULL,
                                     upper_bound IN NUMBER DEFAULT NULL,
	                             tolerance   IN NUMBER DEFAULT NULL)

   RETURN MDSYS.SDO_DIM_ARRAY PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(convert_to_lrs_dim_array, wnds, rnps, wnps);


   FUNCTION convert_to_lrs_dim_array(dim_array   IN MDSYS.SDO_DIM_ARRAY,
			             lower_bound IN NUMBER DEFAULT NULL,
                                     upper_bound IN NUMBER DEFAULT NULL,
	                             tolerance   IN NUMBER DEFAULT NULL)

   RETURN MDSYS.SDO_DIM_ARRAY PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(convert_to_lrs_dim_array, wnds, rnps, wnps);




   FUNCTION convert_to_lrs_dim_array_3d(dim_array   IN MDSYS.SDO_DIM_ARRAY,
				        dim_name    IN VARCHAR2,
			                lower_bound IN NUMBER DEFAULT NULL,
                                        upper_bound IN NUMBER DEFAULT NULL,
	                                tolerance   IN NUMBER DEFAULT NULL)

   RETURN MDSYS.SDO_DIM_ARRAY PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(convert_to_lrs_dim_array_3d, wnds, rnps, wnps);



   FUNCTION convert_to_std_dim_array(dim_array   IN MDSYS.SDO_DIM_ARRAY)
   RETURN MDSYS.SDO_DIM_ARRAY PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(convert_to_std_dim_array, wnds, rnps, wnps);

   FUNCTION convert_to_std_dim_array(dim_array   IN MDSYS.SDO_DIM_ARRAY,
				     m_pos       IN INTEGER)
   RETURN MDSYS.SDO_DIM_ARRAY PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(convert_to_std_dim_array, wnds, rnps, wnps);


   FUNCTION convert_to_std_dim_array_3d(dim_array   IN MDSYS.SDO_DIM_ARRAY)
   RETURN MDSYS.SDO_DIM_ARRAY PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(convert_to_std_dim_array_3d, wnds, rnps, wnps);




   --
   -- Convert standard linestring geometry to an LRS geom segment
   --

   FUNCTION convert_to_lrs_geom(standard_geom IN MDSYS.SDO_GEOMETRY,
				dim_array     IN MDSYS.SDO_DIM_ARRAY,
				m_pos         IN INTEGER,
				start_measure IN NUMBER ,
				end_measure   IN NUMBER )
   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   -- PRAGMA RESTRICT_REFERENCES(convert_to_lrs_geom, wnds, rnps, wnps);

   FUNCTION convert_to_lrs_geom(standard_geom IN MDSYS.SDO_GEOMETRY,
				dim_array     IN MDSYS.SDO_DIM_ARRAY,
				m_pos         IN INTEGER DEFAULT NULL)

   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   -- PRAGMA RESTRICT_REFERENCES(convert_to_lrs_geom, wnds, rnps, wnps);


   FUNCTION convert_to_lrs_geom(standard_geom IN MDSYS.SDO_GEOMETRY,
				dim_array     IN MDSYS.SDO_DIM_ARRAY,
				start_measure IN NUMBER ,
				end_measure   IN NUMBER )
   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   -- PRAGMA RESTRICT_REFERENCES(convert_to_lrs_geom, wnds, rnps, wnps);

   FUNCTION convert_to_lrs_geom(standard_geom IN MDSYS.SDO_GEOMETRY,
				m_pos	      IN INTEGER,
				start_measure IN NUMBER,
				end_measure   IN NUMBER)

   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   -- PRAGMA RESTRICT_REFERENCES(convert_to_lrs_geom, wnds, rnps, wnps);

   FUNCTION convert_to_lrs_geom(standard_geom IN MDSYS.SDO_GEOMETRY,
				start_measure IN NUMBER,
				end_measure   IN NUMBER)

   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   -- PRAGMA RESTRICT_REFERENCES(convert_to_lrs_geom, wnds, rnps, wnps);

   FUNCTION convert_to_lrs_geom(standard_geom IN MDSYS.SDO_GEOMETRY,
				m_pos         IN INTEGER)

   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   -- PRAGMA RESTRICT_REFERENCES(convert_to_lrs_geom, wnds, rnps, wnps);



   FUNCTION convert_to_lrs_geom(standard_geom IN MDSYS.SDO_GEOMETRY)

   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   -- PRAGMA RESTRICT_REFERENCES(convert_to_lrs_geom, wnds, rnps, wnps);



   FUNCTION convert_to_lrs_geom_3d(standard_geom IN MDSYS.SDO_GEOMETRY,
				   dim_array     IN MDSYS.SDO_DIM_ARRAY,
				   start_measure IN NUMBER DEFAULT NULL,
				   end_measure   IN NUMBER DEFAULT NULL)
   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   -- PRAGMA RESTRICT_REFERENCES(convert_to_lrs_geom_3d, wnds, rnps, wnps);

   FUNCTION convert_to_lrs_geom_3d(standard_geom IN MDSYS.SDO_GEOMETRY,
				   start_measure IN NUMBER DEFAULT NULL,
				   end_measure   IN NUMBER DEFAULT NULL)
   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   -- PRAGMA RESTRICT_REFERENCES(convert_to_lrs_geom_3d, wnds, rnps, wnps);



   --
   -- Convert an LRS geometry to a standard linestring geometry
   --

   FUNCTION convert_to_std_geom(lrs_geom IN MDSYS.SDO_GEOMETRY,
			        dim_array     IN MDSYS.SDO_DIM_ARRAY)

   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(convert_to_std_geom, wnds, rnps, wnps);

   FUNCTION convert_to_std_geom(lrs_geom IN MDSYS.SDO_GEOMETRY)

   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(convert_to_std_geom, wnds, rnps, wnps);



   FUNCTION convert_to_std_geom_3d(lrs_geom      IN MDSYS.SDO_GEOMETRY,
			           dim_array     IN MDSYS.SDO_DIM_ARRAY)
   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(convert_to_std_geom_3d, wnds, rnps, wnps);

   FUNCTION convert_to_std_geom_3d(lrs_geom IN MDSYS.SDO_GEOMETRY)
   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(convert_to_std_geom_3d, wnds, rnps, wnps);




   --
   -- Update standard geometry layer to an LRS geometry layer, metadata is updated as well
   --

   FUNCTION convert_to_lrs_layer(table_name    IN VARCHAR2,
                                 column_name   IN VARCHAR2,
                                 lower_bound   IN NUMBER DEFAULT NULL,
                                 upper_bound   IN NUMBER DEFAULT NULL,
                                 tolerance     IN NUMBER DEFAULT NULL)
   RETURN VARCHAR2 PARALLEL_ENABLE;


   FUNCTION convert_to_lrs_layer(table_name    IN VARCHAR2,
                                 column_name   IN VARCHAR2,
			         dim_name      IN VARCHAR2,
				 dim_pos       IN INTEGER,
                                 lower_bound   IN NUMBER DEFAULT NULL,
                                 upper_bound   IN NUMBER DEFAULT NULL,
                                 tolerance     IN NUMBER DEFAULT NULL)
   RETURN VARCHAR2 PARALLEL_ENABLE;



   FUNCTION convert_to_lrs_layer_3d(table_name    IN VARCHAR2,
                                    column_name   IN VARCHAR2,
                                    lower_bound   IN NUMBER DEFAULT NULL,
                                    upper_bound   IN NUMBER DEFAULT NULL,
                                    tolerance     IN NUMBER DEFAULT NULL)
   RETURN VARCHAR2 PARALLEL_ENABLE;


   FUNCTION convert_to_lrs_layer_3d(table_name    IN VARCHAR2,
                                    column_name   IN VARCHAR2,
				    dim_name	  IN VARCHAR2,
                                    lower_bound   IN NUMBER DEFAULT NULL,
                                    upper_bound   IN NUMBER DEFAULT NULL,
                                    tolerance     IN NUMBER DEFAULT NULL)
   RETURN VARCHAR2 PARALLEL_ENABLE;



   --
   -- Update standard geometry layer to an LRS geometry layer, metadata is updated as well
   --

   FUNCTION convert_to_std_layer(table_name    IN VARCHAR2,
                                 column_name   IN VARCHAR2)
   RETURN VARCHAR2 PARALLEL_ENABLE;


   FUNCTION convert_to_std_layer_3d(table_name    IN VARCHAR2,
                                    column_name   IN VARCHAR2)
   RETURN VARCHAR2 PARALLEL_ENABLE;



   --
   -- LRS 817 feature
   -- return a new geom_segment by translateing the original geom_segment
   --

   FUNCTION translate_measure(geom_segment IN MDSYS.SDO_GEOMETRY,
			      dim_array    IN MDSYS.SDO_DIM_ARRAY,
                              translate_m  IN NUMBER)

   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA restrict_references(translate_measure, wnds, rnps, wnps);

   FUNCTION translate_measure(geom_segment IN MDSYS.SDO_GEOMETRY,
                              translate_m  IN NUMBER)

   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA restrict_references(translate_measure, wnds, rnps, wnps);


   FUNCTION translate_measure_3d(geom_segment IN MDSYS.SDO_GEOMETRY,
			         dim_array    IN MDSYS.SDO_DIM_ARRAY,
                                 translate_m  IN NUMBER)

   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA restrict_references(translate_measure_3d, wnds, rnps, wnps);

   FUNCTION translate_measure_3d(geom_segment IN MDSYS.SDO_GEOMETRY,
                                 translate_m  IN NUMBER)

   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA restrict_references(translate_measure_3d, wnds, rnps, wnps);



   --
   -- LRS 817 feature
   -- return a new geom_segment by reversing the original geom segment
   --

   FUNCTION reverse_measure(geom_segment IN MDSYS.SDO_GEOMETRY,
			    dim_array    IN MDSYS.SDO_DIM_ARRAY)

   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA restrict_references(reverse_measure, wnds, rnps, wnps);

   FUNCTION reverse_measure(geom_segment IN MDSYS.SDO_GEOMETRY)

   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA restrict_references(reverse_measure, wnds, rnps, wnps);


   FUNCTION reverse_measure_3d(geom_segment IN MDSYS.SDO_GEOMETRY,
			       dim_array    IN MDSYS.SDO_DIM_ARRAY)

   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA restrict_references(reverse_measure_3d, wnds, rnps, wnps);

   FUNCTION reverse_measure_3d(geom_segment IN MDSYS.SDO_GEOMETRY)

   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA restrict_references(reverse_measure_3d, wnds, rnps, wnps);


   --
   -- LRS 817 find the measure of the closest pt of a given pt w.r.t. an LRS segment
   -- first project the pt onto the LRS segment then get the measure of the projection pt
   --

   FUNCTION find_measure(geom_segment 		IN MDSYS.SDO_GEOMETRY,
  			 dim_array    		IN MDSYS.SDO_DIM_ARRAY,
                         point        		IN MDSYS.SDO_GEOMETRY,
                         point_dim_array	IN MDSYS.SDO_DIM_ARRAY DEFAULT NULL)

   RETURN NUMBER PARALLEL_ENABLE;
   PRAGMA restrict_references(find_measure, wnds, rnps, wnps);



   FUNCTION find_measure(geom_segment IN MDSYS.SDO_GEOMETRY,
                         point        IN MDSYS.SDO_GEOMETRY,
                         tolerance    IN NUMBER DEFAULT 1.0e-8)

   RETURN NUMBER PARALLEL_ENABLE;
   PRAGMA restrict_references(find_measure, wnds, rnps, wnps);


   FUNCTION find_measure_3d(geom_segment 	IN MDSYS.SDO_GEOMETRY,
  			    dim_array    	IN MDSYS.SDO_DIM_ARRAY,
                            point        	IN MDSYS.SDO_GEOMETRY,
                            point_dim_array 	IN MDSYS.SDO_DIM_ARRAY DEFAULT NULL)

   RETURN NUMBER PARALLEL_ENABLE;
   PRAGMA restrict_references(find_measure_3d, wnds, rnps, wnps);

   FUNCTION find_measure_3d(geom_segment IN MDSYS.SDO_GEOMETRY,
                            point        IN MDSYS.SDO_GEOMETRY,
			    tolerance    IN NUMBER DEFAULT 1.0e-8)

   RETURN NUMBER PARALLEL_ENABLE;
   PRAGMA restrict_references(find_measure_3d, wnds, rnps, wnps);



   FUNCTION find_offset(geom_segment 		IN MDSYS.SDO_GEOMETRY,
  			dim_array    		IN MDSYS.SDO_DIM_ARRAY,
                        point        		IN MDSYS.SDO_GEOMETRY,
                        point_dim_array	        IN MDSYS.SDO_DIM_ARRAY DEFAULT NULL)

   RETURN NUMBER PARALLEL_ENABLE;
   PRAGMA restrict_references(find_offset, wnds, rnps, wnps);



   FUNCTION find_offset(geom_segment IN MDSYS.SDO_GEOMETRY,
                        point        IN MDSYS.SDO_GEOMETRY,
                        tolerance    IN NUMBER DEFAULT 1.0e-8)

   RETURN NUMBER PARALLEL_ENABLE;
   PRAGMA restrict_references(find_offset, wnds, rnps, wnps);




   --
   -- LRS 817 dynamic segmentation, an alias of clip_geom_segment
   --


   FUNCTION dynamic_segment(geom_segment  IN MDSYS.SDO_GEOMETRY,
			    dim_array     IN MDSYS.SDO_DIM_ARRAY,
                            start_measure IN NUMBER,
                            end_measure   IN NUMBER )

   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA restrict_references(dynamic_segment, wnds, rnps, wnps);

   FUNCTION dynamic_segment(geom_segment  IN MDSYS.SDO_GEOMETRY,
                            start_measure IN NUMBER,
                            end_measure   IN NUMBER,
                            tolerance     IN NUMBER DEFAULT 1.0e-8)

   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA restrict_references(dynamic_segment, wnds, rnps, wnps);

   FUNCTION dynamic_segment_3d(geom_segment  IN MDSYS.SDO_GEOMETRY,
			       dim_array     IN MDSYS.SDO_DIM_ARRAY,
                               start_measure IN NUMBER,
                               end_measure   IN NUMBER )

   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA restrict_references(dynamic_segment_3d, wnds, rnps, wnps);

   FUNCTION dynamic_segment_3d(geom_segment  IN MDSYS.SDO_GEOMETRY,
                               start_measure IN NUMBER,
                               end_measure   IN NUMBER,
                               tolerance     IN NUMBER DEFAULT 1.0e-8)

   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA restrict_references(dynamic_segment_3d, wnds, rnps, wnps);



   --
   -- LRS 817 convert a measure to a percentage (0%~100%) w.r.t. a given geom segment
   --


   FUNCTION measure_to_percentage(geom_segment IN MDSYS.SDO_GEOMETRY,
  			          dim_array    IN MDSYS.SDO_DIM_ARRAY,
                                  measure      IN NUMBER)

   RETURN NUMBER PARALLEL_ENABLE;
   PRAGMA restrict_references(measure_to_percentage, wnds, rnps, wnps);

   FUNCTION measure_to_percentage(geom_segment IN MDSYS.SDO_GEOMETRY,
                                  measure      IN NUMBER)

   RETURN NUMBER PARALLEL_ENABLE;
   PRAGMA restrict_references(measure_to_percentage, wnds, rnps, wnps);



   --
   -- LRS 817 convert a percentage (0%~100%) to a measure w.r.t. given geom segment
   --

   FUNCTION percentage_to_measure(geom_segment IN MDSYS.SDO_GEOMETRY,
  			          dim_array    IN MDSYS.SDO_DIM_ARRAY,
                                  percentage   IN NUMBER)

   RETURN NUMBER PARALLEL_ENABLE;
   PRAGMA restrict_references(percentage_to_measure, wnds, rnps, wnps);


   FUNCTION percentage_to_measure(geom_segment IN MDSYS.SDO_GEOMETRY,
                                  percentage   IN NUMBER)

   RETURN NUMBER PARALLEL_ENABLE;
   PRAGMA restrict_references(percentage_to_measure, wnds, rnps, wnps);


   --
   -- reset the measure values to NULL
   --
   PROCEDURE reset_measure(geom_segment  IN OUT MDSYS.SDO_GEOMETRY,
   			   dim_array     IN MDSYS.SDO_DIM_ARRAY);
   PRAGMA RESTRICT_REFERENCES(reset_measure, wnds, rnps, wnps);

   PROCEDURE reset_measure(geom_segment  IN OUT MDSYS.SDO_GEOMETRY);
   PRAGMA RESTRICT_REFERENCES(reset_measure, wnds, rnps, wnps);


   --
   -- Redefine the LRS segment (ignore all current measures) and repopulate measure info. based on
   -- the given start/end measure, this is different from define_geom_segment which keeps old measure info
   -- if exist.
   --

   PROCEDURE redefine_geom_segment(geom_segment  IN OUT MDSYS.SDO_GEOMETRY,
   			           dim_array     IN MDSYS.SDO_DIM_ARRAY,
   			           start_measure IN NUMBER ,
   			           end_measure   IN NUMBER );
   --PRAGMA RESTRICT_REFERENCES(redefine_geom_segment, wnds, rnps, wnps);

   PROCEDURE redefine_geom_segment(geom_segment  IN OUT MDSYS.SDO_GEOMETRY,
   			           dim_array     IN MDSYS.SDO_DIM_ARRAY);
   --PRAGMA RESTRICT_REFERENCES(redefine_geom_segment, wnds, rnps, wnps);



   PROCEDURE redefine_geom_segment(geom_segment  IN OUT MDSYS.SDO_GEOMETRY,
   			           start_measure IN NUMBER ,
   			           end_measure   IN NUMBER ,
                                   tolerance     IN NUMBER DEFAULT 1.0e-8);
   --PRAGMA RESTRICT_REFERENCES(redefine_geom_segment, wnds, rnps, wnps);


   PROCEDURE redefine_geom_segment(geom_segment  IN OUT MDSYS.SDO_GEOMETRY,
                                   tolerance     IN NUMBER DEFAULT 1.0e-8);
   --PRAGMA RESTRICT_REFERENCES(redefine_geom_segment, wnds, rnps, wnps);



   PROCEDURE redefine_geom_segment_3d(geom_segment  IN OUT MDSYS.SDO_GEOMETRY,
   			              dim_array     IN MDSYS.SDO_DIM_ARRAY,
   			              start_measure IN NUMBER ,
   			              end_measure   IN NUMBER );
   -- PRAGMA RESTRICT_REFERENCES(redefine_geom_segment_3d, wnds, rnps, wnps);

   PROCEDURE redefine_geom_segment_3d(geom_segment  IN OUT MDSYS.SDO_GEOMETRY,
   			              dim_array     IN MDSYS.SDO_DIM_ARRAY);

   -- PRAGMA RESTRICT_REFERENCES(redefine_geom_segment_3d, wnds, rnps, wnps);

   PROCEDURE redefine_geom_segment_3d(geom_segment  IN OUT MDSYS.SDO_GEOMETRY,
   			              start_measure IN NUMBER ,
   			              end_measure   IN NUMBER ,
                                      tolerance     IN NUMBER DEFAULT 1.0e-8);
   -- PRAGMA RESTRICT_REFERENCES(redefine_geom_segment_3d, wnds, rnps, wnps);

   PROCEDURE redefine_geom_segment_3d(geom_segment  IN OUT MDSYS.SDO_GEOMETRY,
                                      tolerance     IN NUMBER DEFAULT 1.0e-8);
   -- PRAGMA RESTRICT_REFERENCES(redefine_geom_segment_3d, wnds, rnps, wnps);




---
--- 82 LRS Features
---


   --
   -- set the measure value of the closest point (using snap_to_pt) in the given geometry
   --

   FUNCTION set_pt_measure(geom_segment  IN OUT MDSYS.SDO_GEOMETRY,
 		           dim_array     IN MDSYS.SDO_DIM_ARRAY,
			   point         IN MDSYS.SDO_GEOMETRY,
 		           pt_dim_array  IN MDSYS.SDO_DIM_ARRAY,
			   measure       IN NUMBER )
   RETURN VARCHAR2 PARALLEL_ENABLE;
  -- PRAGMA RESTRICT_REFERENCES(set_pt_measure, wnds, rnps, wnps);

   FUNCTION  set_pt_measure(point         IN OUT MDSYS.SDO_GEOMETRY,
 		            dim_array     IN MDSYS.SDO_DIM_ARRAY,
			    measure       IN NUMBER )
   RETURN VARCHAR2 PARALLEL_ENABLE;
  -- PRAGMA RESTRICT_REFERENCES(set_pt_measure, wnds, rnps, wnps);

   FUNCTION  set_pt_measure(point         IN OUT MDSYS.SDO_GEOMETRY,
			    measure       IN NUMBER )
   RETURN VARCHAR2 PARALLEL_ENABLE;
  -- PRAGMA RESTRICT_REFERENCES(set_pt_measure, wnds, rnps, wnps);


   FUNCTION set_pt_measure(geom_segment  IN OUT MDSYS.SDO_GEOMETRY,
			   point         IN MDSYS.SDO_GEOMETRY,
			   measure       IN NUMBER )
   RETURN VARCHAR2 PARALLEL_ENABLE;
  -- PRAGMA RESTRICT_REFERENCES(set_pt_measure, wnds, rnps, wnps);



   FUNCTION set_pt_measure_3d(geom_segment  IN OUT MDSYS.SDO_GEOMETRY,
 		  	      dim_array     IN MDSYS.SDO_DIM_ARRAY,
			      point         IN MDSYS.SDO_GEOMETRY,
 		  	      pt_dim_array  IN MDSYS.SDO_DIM_ARRAY,
			      measure       IN NUMBER )
   RETURN VARCHAR2 PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(set_pt_measure_3d, wnds, rnps, wnps);

   FUNCTION set_pt_measure_3d(geom_segment  IN OUT MDSYS.SDO_GEOMETRY,
			      point         IN MDSYS.SDO_GEOMETRY,
			      measure       IN NUMBER )
   RETURN VARCHAR2 PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(set_pt_measure_3d, wnds, rnps, wnps);


   --
   -- reverse the direction of the geometry (shape pt order )
   --

   FUNCTION reverse_geometry(geom          IN MDSYS.SDO_GEOMETRY,
		             dim_array     IN MDSYS.SDO_DIM_ARRAY)

   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA restrict_references(reverse_geometry, wnds, rnps, wnps);

   FUNCTION reverse_geometry(geom          IN MDSYS.SDO_GEOMETRY)

   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA restrict_references(reverse_geometry, wnds, rnps, wnps);



   --
   -- generate an offset curve w.r.t. to an LRS geometry
   --

   -- for geodetic offset, consider arc_tolerance

   FUNCTION offset_geom_segment(geom_segment  IN MDSYS.SDO_GEOMETRY,
	                        dim_array     IN MDSYS.SDO_DIM_ARRAY,
				start_measure IN NUMBER,
				end_measure   IN NUMBER,
				offset        IN NUMBER,
				unit          IN VARCHAR2)


   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   -- PRAGMA restrict_references(offset_geom_segment, wnds, rnps, wnps);


   -- for geodetic offset, consider arc_tolerance

   FUNCTION offset_geom_segment(geom_segment  IN MDSYS.SDO_GEOMETRY,
				start_measure IN NUMBER,
				end_measure   IN NUMBER,
				offset        IN NUMBER,
				tolerance     IN NUMBER,
				unit          IN VARCHAR2)


   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   -- PRAGMA restrict_references(offset_geom_segment, wnds, rnps, wnps);


   FUNCTION offset_geom_segment(geom_segment  IN MDSYS.SDO_GEOMETRY,
	                        dim_array     IN MDSYS.SDO_DIM_ARRAY,
				start_measure IN NUMBER,
				end_measure   IN NUMBER,
				offset        IN NUMBER)


   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   -- PRAGMA restrict_references(offset_geom_segment, wnds, rnps, wnps);


   FUNCTION offset_geom_segment(geom_segment  IN MDSYS.SDO_GEOMETRY,
				start_measure IN NUMBER,
				end_measure   IN NUMBER,
				offset        IN NUMBER,
				tolerance     IN NUMBER)


   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   -- PRAGMA restrict_references(offset_geom_segment, wnds, rnps, wnps);

   FUNCTION offset_geom_segment(geom_segment  IN MDSYS.SDO_GEOMETRY,
				start_measure IN NUMBER,
				end_measure   IN NUMBER,
				offset        IN NUMBER)


   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   -- PRAGMA restrict_references(offset_geom_segment, wnds, rnps, wnps);


   FUNCTION is_measure_increasing(geom_segment  IN MDSYS.SDO_GEOMETRY,
                                  dim_array     IN MDSYS.SDO_DIM_ARRAY)


   RETURN VARCHAR2 PARALLEL_ENABLE;
   PRAGMA restrict_references(is_measure_increasing, wnds, rnps, wnps);


   FUNCTION is_measure_increasing(geom_segment  IN MDSYS.SDO_GEOMETRY)

   RETURN VARCHAR2 PARALLEL_ENABLE;
   PRAGMA restrict_references(is_measure_increasing, wnds, rnps, wnps);

   FUNCTION is_measure_increasing_3d(geom_segment  IN MDSYS.SDO_GEOMETRY,
                                     dim_array     IN MDSYS.SDO_DIM_ARRAY)


   RETURN VARCHAR2 PARALLEL_ENABLE;
   PRAGMA restrict_references(is_measure_increasing_3d, wnds, rnps, wnps);


   FUNCTION is_measure_increasing_3d(geom_segment  IN MDSYS.SDO_GEOMETRY)

   RETURN VARCHAR2 PARALLEL_ENABLE;
   PRAGMA restrict_references(is_measure_increasing_3d, wnds, rnps, wnps);



   FUNCTION is_measure_decreasing(geom_segment  IN MDSYS.SDO_GEOMETRY,
                                  dim_array     IN MDSYS.SDO_DIM_ARRAY)


   RETURN VARCHAR2 PARALLEL_ENABLE;
   PRAGMA restrict_references(is_measure_decreasing, wnds, rnps, wnps);

   FUNCTION is_measure_decreasing(geom_segment  IN MDSYS.SDO_GEOMETRY)

   RETURN VARCHAR2 PARALLEL_ENABLE;
   PRAGMA restrict_references(is_measure_decreasing, wnds, rnps, wnps);

   FUNCTION is_measure_decreasing_3d(geom_segment  IN MDSYS.SDO_GEOMETRY,
                                     dim_array     IN MDSYS.SDO_DIM_ARRAY)


   RETURN VARCHAR2 PARALLEL_ENABLE;
   PRAGMA restrict_references(is_measure_decreasing_3d, wnds, rnps, wnps);

   FUNCTION is_measure_decreasing_3d(geom_segment  IN MDSYS.SDO_GEOMETRY)

   RETURN VARCHAR2 PARALLEL_ENABLE;
   PRAGMA restrict_references(is_measure_decreasing_3d, wnds, rnps, wnps);


   FUNCTION validate_lrs_geometry(geom_segment  IN MDSYS.SDO_GEOMETRY,
				  dim_array     IN MDSYS.SDO_DIM_ARRAY)

   RETURN VARCHAR2 PARALLEL_ENABLE;
   PRAGMA restrict_references(validate_lrs_geometry, wnds, rnps, wnps);

   FUNCTION validate_lrs_geometry(geom_segment  IN MDSYS.SDO_GEOMETRY)
   RETURN VARCHAR2 PARALLEL_ENABLE;
   PRAGMA restrict_references(validate_lrs_geometry, wnds, rnps, wnps);

   FUNCTION validate_lrs_geometry_3d(geom_segment  IN MDSYS.SDO_GEOMETRY,
				     dim_array     IN MDSYS.SDO_DIM_ARRAY)

   RETURN VARCHAR2 PARALLEL_ENABLE;
   PRAGMA restrict_references(validate_lrs_geometry_3d, wnds, rnps, wnps);

   FUNCTION validate_lrs_geometry_3d(geom_segment  IN MDSYS.SDO_GEOMETRY)
   RETURN VARCHAR2 PARALLEL_ENABLE;
   PRAGMA restrict_references(validate_lrs_geometry_3d, wnds, rnps, wnps);

   FUNCTION find_lrs_dim_pos(table_name    IN VARCHAR2,
			     column_name   IN VARCHAR2)
   RETURN INTEGER PARALLEL_ENABLE;
--   PRAGMA restrict_references(find_lrs_dim_pos, wnds, rnps, wnps);

   --
   -- get previous shape point based on the given measure
   --

   FUNCTION get_prev_shape_pt (geom_segment  IN MDSYS.SDO_GEOMETRY,
			       dim_array     IN MDSYS.SDO_DIM_ARRAY,
			       measure 	     IN NUMBER)

   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(get_prev_shape_pt, wnds, rnps, wnps);


   FUNCTION get_prev_shape_pt (geom_segment  IN MDSYS.SDO_GEOMETRY,
			       measure 	     IN NUMBER)

   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(get_prev_shape_pt, wnds, rnps, wnps);


   FUNCTION get_prev_shape_pt_3d (geom_segment  IN MDSYS.SDO_GEOMETRY,
			          dim_array     IN MDSYS.SDO_DIM_ARRAY,
			          measure       IN NUMBER)

   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(get_prev_shape_pt_3d, wnds, rnps, wnps);


   FUNCTION get_prev_shape_pt_3d (geom_segment  IN MDSYS.SDO_GEOMETRY,
			          measure       IN NUMBER)

   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(get_prev_shape_pt_3d, wnds, rnps, wnps);


   --
   -- get previous shape point based on the given point
   --


   FUNCTION get_prev_shape_pt (geom_segment  IN MDSYS.SDO_GEOMETRY,
			       dim_array     IN MDSYS.SDO_DIM_ARRAY,
			       point	     IN MDSYS.SDO_GEOMETRY)

   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(get_prev_shape_pt, wnds, rnps, wnps);


   FUNCTION get_prev_shape_pt (geom_segment  IN MDSYS.SDO_GEOMETRY,
			       point	     IN MDSYS.SDO_GEOMETRY)

   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(get_prev_shape_pt, wnds, rnps, wnps);


   FUNCTION get_prev_shape_pt_3d (geom_segment  IN MDSYS.SDO_GEOMETRY,
			          dim_array     IN MDSYS.SDO_DIM_ARRAY,
			          point	        IN MDSYS.SDO_GEOMETRY)

   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(get_prev_shape_pt_3d, wnds, rnps, wnps);


   FUNCTION get_prev_shape_pt_3d (geom_segment  IN MDSYS.SDO_GEOMETRY,
			          point	        IN MDSYS.SDO_GEOMETRY)

   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(get_prev_shape_pt_3d, wnds, rnps, wnps);



   --
   -- get the next shape point based on the given measure
   --


   FUNCTION get_next_shape_pt (geom_segment  IN MDSYS.SDO_GEOMETRY,
			       dim_array     IN MDSYS.SDO_DIM_ARRAY,
			       measure 	     IN NUMBER)

   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(get_next_shape_pt, wnds, rnps, wnps);

   FUNCTION get_next_shape_pt (geom_segment  IN MDSYS.SDO_GEOMETRY,
			       measure 	     IN NUMBER)

   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(get_next_shape_pt, wnds, rnps, wnps);


   FUNCTION get_next_shape_pt_3d (geom_segment  IN MDSYS.SDO_GEOMETRY,
			          dim_array     IN MDSYS.SDO_DIM_ARRAY,
			          measure       IN NUMBER)

   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(get_next_shape_pt_3d, wnds, rnps, wnps);

   FUNCTION get_next_shape_pt_3d (geom_segment  IN MDSYS.SDO_GEOMETRY,
			          measure       IN NUMBER)

   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(get_next_shape_pt_3d, wnds, rnps, wnps);



   --
   -- get the next shape point based on  the given point
   --


   FUNCTION get_next_shape_pt (geom_segment  IN MDSYS.SDO_GEOMETRY,
			       dim_array     IN MDSYS.SDO_DIM_ARRAY,
			       point	     IN MDSYS.SDO_GEOMETRY)

   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(get_next_shape_pt, wnds, rnps, wnps);


   FUNCTION get_next_shape_pt (geom_segment  IN MDSYS.SDO_GEOMETRY,
			       point	     IN MDSYS.SDO_GEOMETRY)

   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(get_next_shape_pt, wnds, rnps, wnps);


   FUNCTION get_next_shape_pt_3d (geom_segment  IN MDSYS.SDO_GEOMETRY,
			          dim_array     IN MDSYS.SDO_DIM_ARRAY,
			          point	        IN MDSYS.SDO_GEOMETRY)

   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(get_next_shape_pt_3d, wnds, rnps, wnps);


   FUNCTION get_next_shape_pt_3d (geom_segment  IN MDSYS.SDO_GEOMETRY,
			          point	        IN MDSYS.SDO_GEOMETRY)

   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(get_next_shape_pt_3d, wnds, rnps, wnps);



   --
   -- get previous measure basd on the given measure
   --


   FUNCTION get_prev_shape_pt_measure(geom_segment  IN MDSYS.SDO_GEOMETRY,
			              dim_array     IN MDSYS.SDO_DIM_ARRAY,
			              measure 	     IN NUMBER)

   RETURN NUMBER PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(get_prev_shape_pt_measure, wnds, rnps, wnps);


   FUNCTION get_prev_shape_pt_measure (geom_segment  IN MDSYS.SDO_GEOMETRY,
			               measure 	     IN NUMBER)

   RETURN NUMBER PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(get_prev_shape_pt_measure, wnds, rnps, wnps);



   FUNCTION get_prev_shape_pt_measure_3d(geom_segment  IN MDSYS.SDO_GEOMETRY,
			                 dim_array     IN MDSYS.SDO_DIM_ARRAY,
			                 measure       IN NUMBER)

   RETURN NUMBER PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(get_prev_shape_pt_measure_3d, wnds, rnps, wnps);


   FUNCTION get_prev_shape_pt_measure_3d (geom_segment  IN MDSYS.SDO_GEOMETRY,
			                  measure       IN NUMBER)

   RETURN NUMBER PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(get_prev_shape_pt_measure_3d, wnds, rnps, wnps);



   --
   -- get previous measure (of shape point) based on the given point
   --


   FUNCTION get_prev_shape_pt_measure(geom_segment  IN MDSYS.SDO_GEOMETRY,
		                      dim_array     IN MDSYS.SDO_DIM_ARRAY,
			              point	     IN MDSYS.SDO_GEOMETRY)

   RETURN NUMBER PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(get_prev_shape_pt_measure, wnds, rnps, wnps);


   FUNCTION get_prev_shape_pt_measure(geom_segment  IN MDSYS.SDO_GEOMETRY,
			              point	     IN MDSYS.SDO_GEOMETRY)

   RETURN NUMBER PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(get_prev_shape_pt_measure, wnds, rnps, wnps);




   FUNCTION get_prev_shape_pt_measure_3d(geom_segment  IN MDSYS.SDO_GEOMETRY,
		                         dim_array     IN MDSYS.SDO_DIM_ARRAY,
			                 point	      IN MDSYS.SDO_GEOMETRY)

   RETURN NUMBER PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(get_prev_shape_pt_measure_3d, wnds, rnps, wnps);


   FUNCTION get_prev_shape_pt_measure_3d(geom_segment  IN MDSYS.SDO_GEOMETRY,
			                 point	      IN MDSYS.SDO_GEOMETRY)

   RETURN NUMBER PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(get_prev_shape_pt_measure_3d, wnds, rnps, wnps);




   --
   -- get the next measure (of shape point) based on the given measure
   --


   FUNCTION get_next_shape_pt_measure (geom_segment  IN MDSYS.SDO_GEOMETRY,
		                       dim_array     IN MDSYS.SDO_DIM_ARRAY,
			               measure       IN NUMBER)

   RETURN NUMBER PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(get_next_shape_pt_measure, wnds, rnps, wnps);

   FUNCTION get_next_shape_pt_measure (geom_segment  IN MDSYS.SDO_GEOMETRY,
			               measure 	     IN NUMBER)

   RETURN NUMBER PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(get_next_shape_pt_measure, wnds, rnps, wnps);

   FUNCTION get_next_shape_pt_measure(geom_segment  IN MDSYS.SDO_GEOMETRY,
		                      dim_array     IN MDSYS.SDO_DIM_ARRAY,
			              point	      IN MDSYS.SDO_GEOMETRY)

   RETURN NUMBER PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(get_next_shape_pt_measure, wnds, rnps, wnps);


   FUNCTION get_next_shape_pt_measure(geom_segment  IN MDSYS.SDO_GEOMETRY,
			              point	      IN MDSYS.SDO_GEOMETRY)

   RETURN NUMBER PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(get_next_shape_pt_measure, wnds, rnps, wnps);


   FUNCTION get_next_shape_pt_measure_3d (geom_segment  IN MDSYS.SDO_GEOMETRY,
		                          dim_array     IN MDSYS.SDO_DIM_ARRAY,
			                  measure       IN NUMBER)

   RETURN NUMBER PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(get_next_shape_pt_measure_3d, wnds, rnps, wnps);

   FUNCTION get_next_shape_pt_measure_3d (geom_segment  IN MDSYS.SDO_GEOMETRY,
			                  measure        IN NUMBER)

   RETURN NUMBER PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(get_next_shape_pt_measure_3d, wnds, rnps, wnps);


   FUNCTION get_next_shape_pt_measure_3d(geom_segment  IN MDSYS.SDO_GEOMETRY,
		                         dim_array     IN MDSYS.SDO_DIM_ARRAY,
			                 point	      IN MDSYS.SDO_GEOMETRY)

   RETURN NUMBER PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(get_next_shape_pt_measure_3d, wnds, rnps, wnps);


   FUNCTION get_next_shape_pt_measure_3d(geom_segment  IN MDSYS.SDO_GEOMETRY,
			                 point	      IN MDSYS.SDO_GEOMETRY)

   RETURN NUMBER;
   PRAGMA RESTRICT_REFERENCES(get_next_shape_pt_measure_3d, wnds, rnps, wnps);

   FUNCTION is_shape_pt_measure(geom_segment  IN MDSYS.SDO_GEOMETRY,
		                dim_array     IN MDSYS.SDO_DIM_ARRAY,
				measure       IN NUMBER)

   RETURN VARCHAR2 PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(is_shape_pt_measure, wnds, rnps, wnps);

   FUNCTION is_shape_pt_measure(geom_segment  IN MDSYS.SDO_GEOMETRY,
				measure       IN NUMBER)

   RETURN VARCHAR2 PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(is_shape_pt_measure, wnds, rnps, wnps);

   FUNCTION is_shape_pt_measure_3d(geom_segment  IN MDSYS.SDO_GEOMETRY,
		                   dim_array     IN MDSYS.SDO_DIM_ARRAY,
				   measure       IN NUMBER)

   RETURN VARCHAR2 PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(is_shape_pt_measure_3d, wnds, rnps, wnps);

   FUNCTION is_shape_pt_measure_3d(geom_segment  IN MDSYS.SDO_GEOMETRY,
				   measure       IN NUMBER)

   RETURN VARCHAR2 PARALLEL_ENABLE;
   PRAGMA RESTRICT_REFERENCES(is_shape_pt_measure_3d, wnds, rnps, wnps);

   --
   -- intersection for LRS geometries
   --

   FUNCTION lrs_intersection( geom_1 IN MDSYS.SDO_GEOMETRY,
  			      dim_array_1    IN MDSYS.SDO_DIM_ARRAY,
  			      geom_2 IN MDSYS.SDO_GEOMETRY,
  			      dim_array_2    IN MDSYS.SDO_DIM_ARRAY)

   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
--   PRAGMA RESTRICT_REFERENCES(lrs_intersection, wnds, rnps, wnps);

   FUNCTION lrs_intersection( geom_1 IN MDSYS.SDO_GEOMETRY,
  			      geom_2 IN MDSYS.SDO_GEOMETRY,
			      tolerance      IN NUMBER)
   RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;
--   PRAGMA RESTRICT_REFERENCES(lrs_intersection, wnds, rnps, wnps);

END sdo_lrs;
/


Prompt Synonym SDO_LRS;
--
-- SDO_LRS  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM SDO_LRS FOR MDSYS.SDO_LRS
/


Prompt Grants on PACKAGE SDO_LRS TO PUBLIC to PUBLIC;
GRANT EXECUTE ON MDSYS.SDO_LRS TO PUBLIC
/
