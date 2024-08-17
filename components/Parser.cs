using testCompiler;

class Parser
{
    static HashSet<TokenType> conditionsOperators = [TokenType.Equal, TokenType.NotEqual, TokenType.GreaterThan, 
    TokenType.GreaterThanOrEqual, TokenType.LessThan, TokenType.LessThanOrEqual];
    private static TokenTreeNode? ReorderBlock(TokenTreeNode? node, int bodyId = 0, TokenTreeNode? newTreeHead = null)
    {
        if (node == null)
            return null;

        var newRoot = new TokenTreeNode(node);

        for (int i = 0; i < node.Children.Count; i++)
        {
            var child = node.Children[i];

            if(child.Value.type == TokenType.Identifier_func)
                child.Value = new(child.Value.value.Split(' ')[1], TokenType.Identifier_func);

            if (conditionsOperators.Contains(child.Value.type))
            {
                // Create a new node for the greater-than operator
                TokenTreeNode conditionNode = new(child);

                // Add previous and next siblings as children
                if (i < node.Children.Count - 1)
                    conditionNode.AddChild(node.Children[i + 1]);

                if (i > 0)
                    conditionNode.AddChild(node.Children[i - 1]);

                newRoot.AddChild(conditionNode);
                i++; // Skip the next sibling
            }

            else if(child.Value.type == TokenType.Keyword && child.Value.value == "return")
            {
                TokenTreeNode retNode = new(child);

                if(i < node.Children.Count -1)
                    retNode.AddChild(node.Children[i+1]);

                newRoot.AddChild(retNode);
                i++;
            }
            else if(child.Value.type == TokenType.Assignment)
            {
                TokenTreeNode assignmentNode = new(child);

                if(i < node.Children.Count -1)
                    assignmentNode.AddChild(node.Children[i + 1]);

                if(i > 0)
                    assignmentNode.AddChild(node.Children[i - 1]);

                i++;
                newRoot.AddChild(assignmentNode);
            }

            else
            {
                if(i != node.Children.Count -1)
                {
                    if(conditionsOperators.Contains(node.Children[i+1].Value.type))
                    {

                    }
                    else if(node.Children[i+1].Value.type == TokenType.Assignment)
                    {

                    }
                    else if(child.Value.type == TokenType.Keyword && child.Value.value == "int")
                    {

                    }
                    else
                    {
                        if(!child.IsArgument)
                            newRoot.AddChild(ReorderBlock(node.Children[i]));
                    }
                }

                else
                {
                    if(!child.IsArgument)
                        newRoot.AddChild(ReorderBlock(node.Children[i]));
                }
            }
        }

        return newRoot;
    }

    private static TokenTreeNode? FixIfs(TokenTreeNode? node)
    {
        if (node == null)
            return null;

        var newRoot = new TokenTreeNode(node);

        for (int i = 0; i < node.Children.Count; i++)
        {
            var child = node.Children[i];

            if (child.Value.type == TokenType.Keyword && child.Value.value == "if")
            {
                // Create a new node for the if statement
                var ifNode = new TokenTreeNode(child);

                // Safely add the next two children if they exist
                if (i + 1 < node.Children.Count)
                    ifNode.AddChild(FixIfs(node.Children[i + 1]));

                if (i + 2 < node.Children.Count)
                    ifNode.AddChild(FixIfs(node.Children[i + 2]));

                newRoot.AddChild(ifNode);
                i += 2; // Skip the next two children
            }

            else
                newRoot.AddChild(FixIfs(child));
        }   

        return newRoot;
    }

    public static TokenTreeNode? Parse(TokenTreeNode head)
    {
        TokenTreeNode? ttn = ReorderBlock(head);
        ttn = FixIfs(ttn);
        return ttn;
    }
}