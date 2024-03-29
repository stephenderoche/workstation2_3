if exists (select * from sysobjects where name = 'se_get_security_mv')
begin
	drop procedure se_get_security_mv
	print 'PROCEDURE: se_get_security_mv dropped'
end
go
/*
declare @security_mv1 float

exec se_get_security_mv @security_mv = @security_mv1 output, @account_id  =93, @security_id = 4083,@market_value_type = 3

print @security_mv1
*/

create procedure [dbo].[se_get_security_mv]  --se_get_security_mv 93,4402,1

(    
      @security_mv numeric(10) output,
      @account_id numeric(10),    
      @security_id numeric(10) ,
	  @market_value_type numeric(10) = 1,
	  @account_market_value float
)     

as   

 declare @total_market_value   float;    
 declare @model_type     int;    
 declare @hierarchy_id    numeric(10);    
 declare @ret_val     int;    
 declare @general_error    int;    
 declare @max_hierarchy_depth  smallint;    
 declare @market_value1		float

begin    

 set nocount on    
 declare @ec__errno int    



  create table      #cp_mdl_ac_positions (    
                              position_type_code      tinyint,     
                              security_id             numeric(10),     
                              quantity                float,     
                              market_value            float );   

  

create table      #cp_mdl_ac_proposed_orders (    

                              position_type_code      tinyint,     
                              security_id             numeric(10),     
                              quantity                float,     
                              market_value            float );   

                                

create table      #cp_mdl_ac_orders (    

                              position_type_code      tinyint,     
                              security_id             numeric(10),     
							  quantity                float,     
							  market_value            float );   

                                

   --get account mv  

     

   --exec psg_get_account_market_value_bbh @total_mv output,@account_id,1.0   

    --execute get_account_market_value   
				--@market_value  = @total_mv output,   
				--@account_id   = @account_id,   
				--@market_value_type = 7,   
				--@currency_id  = 1    

  -- print @total_mv

                                

  --sjd  begins positions  

      insert into #cp_mdl_ac_positions (position_type_code, security_id, quantity, market_value)    

      select  
      positions.position_type_code,  
      positions.security_id,  
      positions.quantity,  
      sum((positions.quantity)    
       * coalesce(price.latest, 0.0) * security.principal_factor * pricing_factor / principal.exchange_rate    
       + (0)    
       + positions.accrued_income / income.exchange_rate)   
      from    
            positions,     
            security,     
            price,    
            currency principal,    
			currency income    

      where    
            positions.security_id = security.security_id and    
            security.security_id = price.security_id and    
            security.principal_currency_id = principal.security_id and    
            coalesce(price.latest, 0.0) * security.principal_factor * pricing_factor <> 0.0 and    
            security.income_currency_id = income.security_id  and  
            positions.account_id = @account_id  
			and positions.security_id = @security_id

       group by positions.position_type_code,  
      positions.security_id,  
      positions.quantity  

        
--select * from #cp_mdl_ac_positions

 --sjd  begins propsed_orders  

      insert into #cp_mdl_ac_proposed_orders (position_type_code, security_id, quantity, market_value)    

      select    
            side.position_type_code,    
            security.security_id,    
            sum(security_sign * ((proposed_orders.quantity )    
                  + coalesce((proposed_orders.market_value )   
                  * principal.exchange_rate / (coalesce(price.latest, 0.0) * pricing_factor * principal_factor), 0))),   
   sum(case     
     when security.minor_asset_code <> 12     
      then security_sign * ((proposed_orders.quantity)    
       * coalesce(price.latest, 0.0) * principal_factor * pricing_factor / principal.exchange_rate    
       + (proposed_orders.market_value)    
       + proposed_orders.accrued_income / income.exchange_rate)    
     else 0 
     end)    
      from    
            proposed_orders,     
            side,    
            security,     
            price,    
            currency principal,    
            currency income    
      where    
            proposed_orders.security_id = security.security_id and    
            proposed_orders.side_code = side.side_code and    
            security.security_id = price.security_id and    
            security.principal_currency_id = principal.security_id and    
            coalesce(price.latest, 0.0) * principal_factor * pricing_factor <> 0.0 and    
            security.income_currency_id = income.security_id  and  
              
            proposed_orders.account_id = @account_id  
			and proposed_orders.security_id = @security_id
   group by side.position_type_code, security.security_id    

      --sjd  begins orders  

      insert into #cp_mdl_ac_orders (position_type_code, security_id, quantity, market_value)    
      select    
	      side.position_type_code,    
          security.security_id,    
          sum(security_sign * ((orders.quantity )    
                  + coalesce((orders.market_value )    
                  * principal.exchange_rate / (coalesce(price.latest, 0.0) * pricing_factor * principal_factor), 0))),  
   sum(case     
     when security.minor_asset_code <> 12     
      then security_sign * ((orders.quantity)    
       * coalesce(price.latest, 0.0) * principal_factor * pricing_factor / principal.exchange_rate    
       + (orders.market_value)    
       + orders.accrued_income / income.exchange_rate)    
     else 0    
     end)    

      from     
            orders,     
            side,    
            security,     
            price,    
            currency principal,    
            currency income    
      where    
            orders.security_id = security.security_id and    
            orders.side_code = side.side_code and    
            security.security_id = price.security_id and    
            security.principal_currency_id = principal.security_id and    
            coalesce(price.latest, 0.0) * principal_factor * pricing_factor <> 0.0 and    
			security.income_currency_id = income.security_id and  
            orders.deleted = 0 and   
            orders.account_id = @account_id  
			and orders.security_id = @security_id
		group by side.position_type_code, security.security_id   

         

      declare @positions_mv numeric(10)  

      select  @positions_mv = sum(coalesce(#cp_mdl_ac_positions.market_value, 0.0))  
      from #cp_mdl_ac_positions  


       declare @proposed_mv float  
       select @proposed_mv = sum(coalesce(#cp_mdl_ac_proposed_orders.market_value, 0.0))  
       from #cp_mdl_ac_proposed_orders  

       declare @orders_mv float  



       select @orders_mv =   sum(coalesce(#cp_mdl_ac_orders.market_value, 0.0))  
       from #cp_mdl_ac_orders  

	

       declare @total_mv_issuer float
	   select @total_mv_issuer = coalesce(@positions_mv,0) +coalesce( @proposed_mv,0) + coalesce(@orders_mv,0)  

  
  if(@market_value_type = 1)
     begin
       set @market_value1  = coalesce(@proposed_mv,0)
	 end
   else if(@market_value_type = 2)
      begin
	   set @market_value1  =  coalesce(@orders_mv,0)
	  end
else if(@market_value_type = 3)
      begin
	   set @market_value1  =  coalesce(@positions_mv,0)
	  end
	else if(@market_value_type = 4)
      begin
	   set @market_value1  =  coalesce(@total_mv_issuer,0)
	  end
	else
	  begin
	    set @market_value1  =  coalesce(@account_market_value,0)
	  end

	 

	  select @security_mv = coalesce(@market_value1,0)
	
	 return

      end  


go
if @@error = 0 print 'PROCEDURE: se_get_security_mv created'
else print 'PROCEDURE: se_get_security_mv error on creation'
go
    
    
