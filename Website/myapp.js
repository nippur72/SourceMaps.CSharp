function test()
{
    var b=0;
    for(var t=1;t<10;t++)
    {
        b=b+t;   
    }
    //window.alert("finished");
}

test();

function factorial(n)
{
    if(n==1) return n; 
    else return n*factorial(n-1);
}

var z=55+66+88+factorial(2);

//debugger;
var f = factorial(6);
//window.alert(f);

//# sourceMappingURL=myapp.js.map