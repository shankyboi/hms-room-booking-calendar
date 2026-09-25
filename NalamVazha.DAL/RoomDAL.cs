namespace NalamVazha.DAL{
			    using System;
			    using System.Text;
			    using System.Data;
			    using System.Data.Common;
			    using NalamVazha.Models;
			    using EncrypDecrypt;
			    using Newtonsoft.Json;
				using Newtonsoft.Json.Linq;
                using Npgsql;
				using NpgsqlTypes;
				using System.Text.RegularExpressions;

			    //This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:22
			    public class RoomDAL
			    {
					public virtual string db_connectionstring{get;set;}
					
			 	    public RoomDAL(string connectionString)
				    {
						
					    db_connectionstring=connectionString;
				    }

					// Real availability based on RoomOccupancyStatus overlap (DB: get_room_list_available)
					public virtual DataTable Get_Room_List_Available(Guid tenantid, Guid? roomtype, DateTime fromdate, DateTime todate)
					{
						DataTable dataTable = new DataTable();
						DataSet dataSet = new DataSet();
						try
						{
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								// NOTE: function name was created without quotes, so PostgreSQL stores it in lowercase.
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM get_room_list_available(@pvar_tenantid,@pvar_roomtype,@pvar_fromdate,@pvar_todate)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_tenantid", NpgsqlDbType.Uuid, (object)tenantid);
									dbCommand.Parameters.AddWithValue("pvar_roomtype", NpgsqlDbType.Uuid, (object)roomtype ?? DBNull.Value);
									dbCommand.Parameters.AddWithValue("pvar_fromdate", NpgsqlDbType.Date, (object)fromdate.Date);
									dbCommand.Parameters.AddWithValue("pvar_todate", NpgsqlDbType.Date, (object)todate.Date);

									using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(dbCommand))
									{
										dataSet.Reset();
										dataAdapter.Fill(dataSet);
										dataTable = dataSet.Tables.Count > 0 ? dataSet.Tables[0] : new DataTable();
										if (dbCommand.Connection.State != ConnectionState.Closed)
										{
											dbCommand.Connection.Dispose();
										}
									}
								}
								npsql.Close();
							}
						}
						catch
						{
							throw;
						}
						return dataTable;
					}
				  
			        
              public virtual string Add_Room(RoomModel model)
			  { 
				  String ResponseMessage="";
					 
					try{
							 
                            using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					        {
						        npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Add_Room\"(@pvar_roomid,@pvar_tenantid,@pvar_roomcode,@pvar_block,@pvar_building,@pvar_floor,@pvar_roomtype,@pvar_costperday,@pvar_advanceperday,@pvar_bookingdeposit,@pvar_attendantcostperday,@pvar_attendantadvanceperday,@pvar_attendantbookingdeposit,@pvar_roomtransfercost,@pvar_roomgroup,@pvar_roomnumber,@pvar_roomimage,@pvar_nextdaycheckin,@pvar_nextdaycheckout,@pvar_createduser,@pvar_easeofaccess)", npsql))
					{
						dbCommand.Parameters.AddWithValue("pvar_roomid", NpgsqlDbType.Uuid, (object)model.Roomid ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_tenantid", NpgsqlDbType.Uuid, (object)model.tenantid ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_roomcode", NpgsqlDbType.Varchar, (object)model.roomcode ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_block", NpgsqlDbType.Uuid, (object)model.block ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_building", NpgsqlDbType.Uuid, (object)model.building ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_floor", NpgsqlDbType.Uuid, (object)model.floor ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_roomtype", NpgsqlDbType.Uuid, (object)model.roomtype ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_costperday", NpgsqlDbType.Numeric, (object)model.costperday ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_advanceperday", NpgsqlDbType.Numeric, (object)model.advanceperday ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_bookingdeposit", NpgsqlDbType.Numeric, (object)model.bookingdeposit ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_attendantcostperday", NpgsqlDbType.Numeric, (object)model.attendantcostperday ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_attendantadvanceperday", NpgsqlDbType.Numeric, (object)model.attendantadvanceperday ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_attendantbookingdeposit", NpgsqlDbType.Numeric, (object)model.attendantbookingdeposit ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_roomtransfercost", NpgsqlDbType.Varchar, (object)model.roomtransfercost ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_roomgroup", NpgsqlDbType.Uuid, (object)model.roomgroup ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_roomnumber", NpgsqlDbType.Varchar, (object)model.roomnumber ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_roomimage", NpgsqlDbType.Varchar, (object)model.roomimage ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_nextdaycheckin", NpgsqlDbType.Varchar, (object)model.nextdaycheckin ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_nextdaycheckout", NpgsqlDbType.Varchar, (object)model.nextdaycheckout ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_createduser", NpgsqlDbType.Uuid, (object)model.createduser ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_easeofaccess", NpgsqlDbType.Varchar, (object)model.easeofaccess ?? DBNull.Value);

						


						NpgsqlParameter outParm = new NpgsqlParameter("pvar_returnMessage", NpgsqlDbType.Varchar)
                                        {
                                             Direction = ParameterDirection.Output
                                        };
                                        dbCommand.Parameters.Add(outParm);

                                        dbCommand.ExecuteNonQuery();
								        ResponseMessage = outParm.Value.ToString();
								        if (dbCommand.Connection.State != ConnectionState.Closed)
                    			        {
										         dbCommand.Connection.Dispose();
								        }

						        }
						        npsql.Close();
					        }
 

					}catch(Exception ex){
						ResponseMessage=ex.Message;
						Console.WriteLine(ex);
					} 
					
					return ResponseMessage;

			   }
		/// <summary>
		/// Returns cost fields for a room via the get_room_cost_fields stored procedure.
		/// Used during receivable generation to get room-level costs instead of roomtype defaults.
		/// </summary>
		public virtual RoomModel GetRoomCostFields(Guid roomId)
		{
			using var conn = new NpgsqlConnection(db_connectionstring);
			conn.Open();
			using var cmd = new NpgsqlCommand(
				"SELECT * FROM public.get_room_cost_fields(@p_roomid)", conn);
			cmd.Parameters.Add("p_roomid", NpgsqlDbType.Uuid).Value = roomId;

			using var reader = cmd.ExecuteReader();
			if (!reader.Read()) return null;

			return new RoomModel
			{
				costperday              = reader.IsDBNull(0) ? (decimal?)null : reader.GetDecimal(0),
				attendantcostperday     = reader.IsDBNull(1) ? (decimal?)null : reader.GetDecimal(1),
				bookingdeposit          = reader.IsDBNull(2) ? (decimal?)null : reader.GetDecimal(2),
				attendantbookingdeposit = reader.IsDBNull(3) ? (decimal?)null : reader.GetDecimal(3),
			};
		}

public virtual RoomModel getById_Room(string Roomid)
									 {
										DataTable dataTable = new DataTable();
										DataSet dataSet = new DataSet();
										try{
												 
												using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
												{
													npsql.Open();
													using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_Room\"(@pvar_roomid)", npsql))
													{
														dbCommand.CommandType = CommandType.Text;
														dbCommand.Parameters.AddWithValue("pvar_roomid",(object)Roomid??DBNull.Value);
														using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(dbCommand))
														{
															dataSet.Reset();
															dataAdapter.Fill(dataSet);
															dataTable = dataSet.Tables[0];
															if (dbCommand.Connection.State != ConnectionState.Closed)
															{
																dbCommand.Connection.Dispose();
															}
														}
													}
													npsql.Close();
												}
					 
										}catch{
												throw;
										}
										if (dataTable.Rows.Count > 0)
										{
											DataRow row = dataTable.Rows[0];
											return ModelConverter.ConvertDataRowToModel<RoomModel>(row);
										}
										else
										{
											return null;
										}
									 }
			 public virtual string  Update_Room(RoomModel model)
			 { 
				 String ResponseMessage="";
					try{
						 	 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Update_Room\"(@pvar_roomid,@pvar_tenantid,@pvar_roomcode,@pvar_block,@pvar_building,@pvar_floor,@pvar_roomtype,@pvar_costperday,@pvar_advanceperday,@pvar_bookingdeposit,@pvar_attendantcostperday,@pvar_attendantadvanceperday,@pvar_attendantbookingdeposit,@pvar_roomtransfercost,@pvar_roomgroup,@pvar_roomnumber,@pvar_roomimage,@pvar_nextdaycheckin,@pvar_nextdaycheckout,@pvar_modifieduser,@pvar_easeofaccess)", npsql))
					{
						dbCommand.CommandType = CommandType.Text;
						dbCommand.Parameters.AddWithValue("pvar_roomid", NpgsqlDbType.Uuid, (object)model.Roomid ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_tenantid", NpgsqlDbType.Uuid, (object)model.tenantid ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_roomcode", NpgsqlDbType.Varchar, (object)model.roomcode ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_block", NpgsqlDbType.Uuid, (object)model.block ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_building", NpgsqlDbType.Uuid, (object)model.building ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_floor", NpgsqlDbType.Uuid, (object)model.floor ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_roomtype", NpgsqlDbType.Uuid, (object)model.roomtype ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_costperday", NpgsqlDbType.Numeric, (object)model.costperday ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_advanceperday", NpgsqlDbType.Numeric, (object)model.advanceperday ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_bookingdeposit", NpgsqlDbType.Numeric, (object)model.bookingdeposit ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_attendantcostperday", NpgsqlDbType.Numeric, (object)model.attendantcostperday ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_attendantadvanceperday", NpgsqlDbType.Numeric, (object)model.attendantadvanceperday ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_attendantbookingdeposit", NpgsqlDbType.Numeric, (object)model.attendantbookingdeposit ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_roomtransfercost", NpgsqlDbType.Varchar, (object)model.roomtransfercost ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_roomgroup", NpgsqlDbType.Uuid, (object)model.roomgroup ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_roomnumber", NpgsqlDbType.Varchar, (object)model.roomnumber ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_roomimage", NpgsqlDbType.Varchar, (object)model.roomimage ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_nextdaycheckin", NpgsqlDbType.Varchar, (object)model.nextdaycheckin ?? DBNull.Value);

						dbCommand.Parameters.AddWithValue("pvar_nextdaycheckout", NpgsqlDbType.Varchar, (object)model.nextdaycheckout ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_modifieduser", NpgsqlDbType.Uuid, model.modifieduser);

						dbCommand.Parameters.AddWithValue("pvar_easeofaccess", NpgsqlDbType.Varchar, (object)model.easeofaccess ?? DBNull.Value);


						NpgsqlParameter outParm = new NpgsqlParameter("@returnMessage", NpgsqlDbType.Varchar)
						{
							Direction = ParameterDirection.Output
						};
						dbCommand.Parameters.Add(outParm);

						dbCommand.ExecuteNonQuery();
						ResponseMessage = outParm.Value.ToString();
						if (dbCommand.Connection.State != ConnectionState.Closed)
						{
							dbCommand.Connection.Dispose();
						}

					}
					npsql.Close();
				}


			}
			catch (Exception ex){
						ResponseMessage=ex.Message;
					}
					
					return ResponseMessage;

			   }
public virtual string  Remove_Room(string id,string loginUserID)
			  { 
				  String ResponseMessage="";
					try{ 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Remove_Room\"(@pvar_roomid,@pvar_modifieduser)", npsql))
								{
										dbCommand.CommandType = CommandType.Text;
										dbCommand.Parameters.AddWithValue("pvar_roomid",(object)id??DBNull.Value);
										dbCommand.Parameters.AddWithValue("pvar_modifieduser",(object)loginUserID??DBNull.Value);
										NpgsqlParameter outParm = new NpgsqlParameter("@returnMessage", NpgsqlDbType.Varchar)
										{
											 Direction = ParameterDirection.Output
										};
										dbCommand.Parameters.Add(outParm);

										dbCommand.ExecuteNonQuery();
										ResponseMessage = outParm.Value.ToString();
										if (dbCommand.Connection.State != ConnectionState.Closed)
										{
												 dbCommand.Connection.Dispose();
										}

								}
								npsql.Close();
							}	 

					}catch(Exception ex){
						ResponseMessage=ex.Message;
					}
					
					return ResponseMessage;

			   }
		public virtual System.Data.DataTable Room_List(string tenantid
		, string block
		, string building
		, string floor
		, string roomtype
		, string roomnumber
		, string nextdaycheckin
		, string nextdaycheckout
		)
		{
			DataTable dataTable = new DataTable();
			DataSet dataSet = new DataSet();

			try
			{

				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"Room_List\"(@pvar_tenantid,@pvar_block,@pvar_building,@pvar_floor,@pvar_roomtype,@pvar_roomnumber,@pvar_nextdaycheckin,@pvar_nextdaycheckout)", npsql))
					{
						dbCommand.CommandType = CommandType.Text;
						dbCommand.Parameters.AddWithValue("pvar_tenantid", (object)tenantid ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_block", (object)block ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_building", (object)building ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_floor", (object)floor ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_roomtype", (object)roomtype ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_roomnumber", (object)roomnumber ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_nextdaycheckin", (object)nextdaycheckin ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_nextdaycheckout", (object)nextdaycheckout ?? DBNull.Value);

						using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(dbCommand))
						{
							dataSet.Reset();
							dataAdapter.Fill(dataSet);
							dataTable = dataSet.Tables[0];
							if (dbCommand.Connection.State != ConnectionState.Closed)
							{
								dbCommand.Connection.Dispose();
							}
						}
					}
					npsql.Close();
				}



			}
			catch
			{
				throw;
			}


			return dataTable;




		}



		public virtual System.Data.DataTable get_all_Room(string tenantid)
			  { 

				    DataTable dataTable = new DataTable();
					DataSet dataSet = new DataSet();

					try{
 
							using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
							{
								npsql.Open();
								using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"get_all_Room\"(@pvar_tenantid)", npsql))
								{
									dbCommand.CommandType = CommandType.Text;
									dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);
									
									using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(dbCommand))
									{
										dataSet.Reset();
										dataAdapter.Fill(dataSet);
										dataTable = dataSet.Tables[0];
										if (dbCommand.Connection.State != ConnectionState.Closed)
										{
											dbCommand.Connection.Dispose();
										}
									}
								}
								npsql.Close();
							}
						

					}catch{
						throw;
					}
					return dataTable;	


					 

			   }
		public virtual System.Data.DataTable count_of_Room_occupancystatus(string tenantid
		, string block
		, string building
		, string floor
		, string roomtype
		, string roomnumber
		)
		{
			DataTable dataTable = new DataTable();
			DataSet dataSet = new DataSet();

			try
			{

				using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
				{
					npsql.Open();
					using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"count_of_Room_occupancystatus\"(@pvar_tenantid,@pvar_block,@pvar_building,@pvar_floor,@pvar_roomtype,@pvar_roomnumber)", npsql))
					{
						dbCommand.CommandType = CommandType.Text;
						dbCommand.Parameters.AddWithValue("pvar_tenantid", (object)tenantid ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_block", (object)block ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_building", (object)building ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_floor", (object)floor ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_roomtype", (object)roomtype ?? DBNull.Value);
						dbCommand.Parameters.AddWithValue("pvar_roomnumber", (object)roomnumber ?? DBNull.Value);

						using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(dbCommand))
						{
							dataSet.Reset();
							dataAdapter.Fill(dataSet);
							dataTable = dataSet.Tables[0];
							if (dbCommand.Connection.State != ConnectionState.Closed)
							{
								dbCommand.Connection.Dispose();
							}
						}
					}
					npsql.Close();
				}



			}
			catch
			{
				throw;
			}


			return dataTable;




		}


		public virtual System.Data.DataTable getById_allinfo_Room(string Roomid)
			 {
				DataSet dataSet=new DataSet();
				DataTable dataTable = new DataTable();
				try{
					     
						using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
						{
							npsql.Open();
							using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"getById_sp_all_Room\"(@pvar_roomid)", npsql))
							{
								dbCommand.CommandType = CommandType.Text;
								dbCommand.Parameters.AddWithValue("pvar_roomid",(object)Roomid??DBNull.Value);
								using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(dbCommand))
								{
									dataSet.Reset();
									dataAdapter.Fill(dataSet);
									dataTable = dataSet.Tables[0];
									if (dbCommand.Connection.State != ConnectionState.Closed)
									{
										dbCommand.Connection.Dispose();
									}
								}
							}
							npsql.Close();
						}
					 
				}catch{
						throw;
				}
				return dataTable;
			 }
			  
public virtual System.Data.DataTable lookup_Room_block(String tenantid)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_Room_block\"(@pvar_tenantid)", npsql))
						                                    {
                                            
                                                                dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);  
																
																 
                                                                dbCommand.CommandType = CommandType.Text;
							                                    using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(dbCommand))
                                                                {
                                                                    dataSet.Reset();
                                                                    dataAdapter.Fill(dataSet);
                                                                    dataTable = dataSet.Tables[0];
                                                                    if (dbCommand.Connection.State != ConnectionState.Closed)
												                    {
													                    dbCommand.Connection.Dispose();
												                    }
                                                                }
						                                    }
						                                    npsql.Close();
					                                    }
			 

										 
									        }catch{
										        throw;
									        }
									        return dataTable;
							        }
public virtual System.Data.DataTable lookup_Room_building(String tenantid,String block)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_Room_building\"(@pvar_tenantid,@pvar_block)", npsql))
						                                    {
                                            
                                                                dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);dbCommand.Parameters.AddWithValue("pvar_block",(object)block??DBNull.Value);  
																
																 
                                                                dbCommand.CommandType = CommandType.Text;
							                                    using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(dbCommand))
                                                                {
                                                                    dataSet.Reset();
                                                                    dataAdapter.Fill(dataSet);
                                                                    dataTable = dataSet.Tables[0];
                                                                    if (dbCommand.Connection.State != ConnectionState.Closed)
												                    {
													                    dbCommand.Connection.Dispose();
												                    }
                                                                }
						                                    }
						                                    npsql.Close();
					                                    }
			 

										 
									        }catch{
										        throw;
									        }
									        return dataTable;
							        }
public virtual System.Data.DataTable lookup_Room_floor(String tenantid,String block,String building)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_Room_floor\"(@pvar_tenantid,@pvar_block,@pvar_building)", npsql))
						                                    {
                                            
                                                                dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);dbCommand.Parameters.AddWithValue("pvar_block",(object)block??DBNull.Value);dbCommand.Parameters.AddWithValue("pvar_building",(object)building??DBNull.Value);  
																
																 
                                                                dbCommand.CommandType = CommandType.Text;
							                                    using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(dbCommand))
                                                                {
                                                                    dataSet.Reset();
                                                                    dataAdapter.Fill(dataSet);
                                                                    dataTable = dataSet.Tables[0];
                                                                    if (dbCommand.Connection.State != ConnectionState.Closed)
												                    {
													                    dbCommand.Connection.Dispose();
												                    }
                                                                }
						                                    }
						                                    npsql.Close();
					                                    }
			 

										 
									        }catch{
										        throw;
									        }
									        return dataTable;
							        }
public virtual System.Data.DataTable lookup_Room_roomtype(String tenantid)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_Room_roomtype\"(@pvar_tenantid)", npsql))
						                                    {
                                            
                                                                dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);  
																
																 
                                                                dbCommand.CommandType = CommandType.Text;
							                                    using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(dbCommand))
                                                                {
                                                                    dataSet.Reset();
                                                                    dataAdapter.Fill(dataSet);
                                                                    dataTable = dataSet.Tables[0];
                                                                    if (dbCommand.Connection.State != ConnectionState.Closed)
												                    {
													                    dbCommand.Connection.Dispose();
												                    }
                                                                }
						                                    }
						                                    npsql.Close();
					                                    }
			 

										 
									        }catch{
										        throw;
									        }
									        return dataTable;
							        }
public virtual System.Data.DataTable lookup_Room_roomgroup(String tenantid)
							        {
                                            DataSet dataSet = new DataSet();
									        DataTable dataTable=new DataTable();
									        try{

                                        		        using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
					                                    {
						                                    npsql.Open();
						                                    using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_Room_roomgroup\"(@pvar_tenantid)", npsql))
						                                    {
                                            
                                                                dbCommand.Parameters.AddWithValue("pvar_tenantid",(object)tenantid??DBNull.Value);  
																
																 
                                                                dbCommand.CommandType = CommandType.Text;
							                                    using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(dbCommand))
                                                                {
                                                                    dataSet.Reset();
                                                                    dataAdapter.Fill(dataSet);
                                                                    dataTable = dataSet.Tables[0];
                                                                    if (dbCommand.Connection.State != ConnectionState.Closed)
												                    {
													                    dbCommand.Connection.Dispose();
												                    }
                                                                }
						                                    }
						                                    npsql.Close();
					                                    }
			 

										 
									        }catch{
										        throw;
									        }
									        return dataTable;
							        }


						public virtual System.Data.DataTable lookup_change_Room_roomtype(string RoomTypeid)
						{
							DataTable dataTable=new DataTable();
                            DataSet dataSet=new DataSet();
							try
							{
								 	 
                                    using (NpgsqlConnection npsql = new NpgsqlConnection(db_connectionstring))
									{
										npsql.Open();
										using (var dbCommand = new NpgsqlCommand("SELECT * FROM \"lookup_change_Room_roomtype\"(@pvar_roomtypeid)", npsql))
										{
											dbCommand.CommandType = CommandType.Text;
											dbCommand.Parameters.AddWithValue("pvar_roomtypeid",NpgsqlDbType.Varchar,(object)RoomTypeid??DBNull.Value);
											using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(dbCommand))
											{
												dataSet.Reset();
												dataAdapter.Fill(dataSet);
												dataTable = dataSet.Tables[0];
												if (dbCommand.Connection.State != ConnectionState.Closed)
												{
													dbCommand.Connection.Dispose();
												}
											}
										}
										npsql.Close();
									}

								 
							}catch{
								throw;
							}

							return dataTable;
						}






			    }


			    }
