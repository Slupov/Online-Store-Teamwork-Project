$("#myLink").click(function(e){

    e.preventDefault();
    $.ajax({

       
    success: function(){
        alert("Value Added");  // or any other indication if you want to show
    }

});

});