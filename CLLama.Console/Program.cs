using LLama;

InteractiveExecutor? executor;
do executor = await LoaderScreen.Show();
while (executor == null);

await ChatScreen.Show(executor);
